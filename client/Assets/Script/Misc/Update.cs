/********************************************************
    id:Update.cs
    desc: 该文件定义资源/版本更新相关的组件
    author: figo
    date: 2019/03/05 10:22:52

    Copyright (C) 2019 zwwx Ltd. All rights reserved.
*********************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.Misc {
    using ZF.Core.Util;
    using ZF.Core.Logging;
    using ILogger = ZF.Core.Logging.ILogger; 
    using HTTP2;

    // A help class of download file use http.
    internal class UpdateFile {
        public readonly string name;
        public readonly string remote_path;
        public readonly string locate_path;
        public readonly int content_length;
        public readonly string md5code;

        private string filename;
        private string tmp_filename;

        protected Request request = null;
        protected bool done = false;
        protected bool error = false;

        private ILogger logger_ = NullLogger.Instance;
        public ILogger logger {
            get { return this.logger_; }
            set {
                if (value == null) throw new ArgumentNullException("logger can't be null");
                this.logger_ = value;
            }
        }

        public UpdateFile (string name, string remote_path, string locate_path)
            : this(name, remote_path, locate_path, 0) { }

        public UpdateFile(string name, string remote_path, string locate_path, int content_length)
            : this(name, remote_path, locate_path, content_length, string.Empty) { }

        public UpdateFile(string name, string remote_path, string locate_path, int content_length, string md5code) {
            this.name = name;
            this.remote_path = remote_path;
            this.locate_path = locate_path;
            this.content_length = content_length;
            this.md5code = md5code;

            this.tmp_filename = string.Concat(this.locate_path, Path.DirectorySeparatorChar, this.name, ".tmp");
            this.filename = string.Concat(this.locate_path, Path.DirectorySeparatorChar, this.name);
        }

        public bool IsDone { get { return done; } }
        public bool HasError { get { return error; } }

        // Current length of downloaded.
        public virtual int Length {
            get {
                if (this.request == null)
                    return 0;
                switch (this.request.state) {
                case RequestState.Reading: if (this.request.response != null) return this.request.response.length; else return 0;
                case RequestState.Done: return this.content_length;
                default:
                    return 0;
                }
            }
        }

        // Start download file async.
        public virtual void Download() {
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            done = error = false;

            var stream = new FileStream(this.tmp_filename, FileMode.Create, FileAccess.ReadWrite);
            this.request = new Request("get", string.Concat(this.remote_path, "/", this.name));
            this.request.acceptGzip = false;
            this.request.outputStream = stream;
            this.request.Send(OnComplete);
            // done = error = false;
        }

        // Start download file sync.
        public void DownloadSync() {
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var stream = new FileStream(this.tmp_filename, FileMode.Create, FileAccess.ReadWrite);
            this.request = new Request("get", string.Concat(this.remote_path, "/", this.name));
            this.request.acceptGzip = false;
            this.request.synchronous = true;
            this.request.outputStream = stream;
            this.request.Send(OnComplete);
        }

        protected virtual void OnComplete(Request request) {
            this.done = true;
            if (request.response == null || !(request.response.status >= 200 && request.response.status < 300)) {
                // error
                error = true;
                return;
            }

            // success
            // md5 verify
            if (!string.IsNullOrEmpty(this.md5code)) {
                using (var fsw = new FileStream(this.tmp_filename, FileMode.Open, FileAccess.Read)) {
                    System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
                    byte[] bytes = md5.ComputeHash(fsw);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < bytes.Length; ++i) { sb.Append(bytes[i].ToString("x2")); }
                    string hashcode = sb.ToString();
                    if (!this.md5code.Equals(hashcode)) {
                        logger.Error("verify file {0} md5 failed: {1} != {2}", this.name, this.md5code, hashcode);
                        //error
                        error = true;
                        return;
                    }
                }
            }

            // save file
            if (filename.EndsWith(".zip")) {
                using (var fsw = new FileStream(this.tmp_filename, FileMode.Open, FileAccess.Read)) {
                    Unzip(Path.GetDirectoryName(this.tmp_filename), fsw);
                }
                File.Delete(this.tmp_filename);
            } else {
                if (File.Exists(filename)) File.Delete(filename);
                FileInfo fi = new FileInfo(this.tmp_filename);
                fi.MoveTo(filename);
            }
        }

        protected bool Unzip(string root, Stream s) {
            throw new NotImplementedException("zip file is unsupported");
            //return false;
        }
    }

    internal class VersionFile : UpdateFile {
        private Updater updater;
        public VersionFile(Updater updater, string name, string remote_path) : base(name, remote_path, "") { 
            this.updater = updater;
        }

        public override void Download() {
            this.done = error = false;
            this.request = new Request("get", string.Concat(this.remote_path, "/", this.name));
            this.request.acceptGzip = false;
            this.request.Send(OnComplete);
        }

        protected override void OnComplete(Request request) {
            this.done = true;
            if (request.response == null || !(request.response.status >= 200 && request.response.status < 300)) {
                // error
                error = true;
                return;
            }
            logger.Info("===> downalod version file: {0}", request.response.Text);
            this.updater.ParseVersion(request.response.Text);
        }
        public override int Length { get { return 0; } }
    }

    internal class ManifestFile : UpdateFile {
        private Updater updater;
        public ManifestFile(Updater updater, string name, string remote_path) 
            : base(name, remote_path, "") {
            this.updater = updater;
        }

        public override void Download() {
            this.done = error = false;
            this.request = new Request("get", string.Concat(this.remote_path, "/", this.name));
            this.request.acceptGzip = false;
            this.request.Send(OnComplete);
        }

        protected override void OnComplete(Request request) {
            this.done = true;
            if (request.response == null || !(request.response.status >= 200 && request.response.status < 300)) {
                // error
                error = true;
                return;
            }
            logger.Info("==> downalod manifest file: {0}/{1}", Path.GetFileName(this.remote_path), request.response.Text);
            this.updater.ParseManifest(request.response.Text, this.remote_path);
        }
        public override int Length { get { return 0; } }
    }

    // 资源/版本更新器
    public class Updater {
        public class ErrorCode {
            public const int CHECK_FAILED = 1;   // 检测版本失败
            public const int INCOMPATIBLE = 2;   // 版本不兼容
            public const int FILE_ERROR = 3;     // 下载文件错误
        }

        private const string MANIFEST = "version.manifest";
        private string cur_version = "0.0.0";
        private string new_version = "0.0.0";

        private ILogger logger_ = NullLogger.Instance;
        public ILogger logger {
            get { return this.logger_; }
            set {
                if (value == null) throw new ArgumentNullException("logger can't be null");
                this.logger_ = value;
            }
        }

        private string url = "";
        private string cdn_url = "";
        private int total_size = 0;

        private Queue<UpdateFile> manifests = new Queue<UpdateFile>();
        private Stack<UpdateFile> files = new Stack<UpdateFile>();
        private HashSet<string> hashs = new HashSet<string>();
        private UpdateFile current = null;

        public Action<int, string> onError;
        public Action<float> onProgress;
        public Action<bool> onComplete;

        public int totalSize { get { return total_size; } }
        public int downloadSize { get { return download_size; } }
        public int downloadSpeed { get { return download_speed; } }
        public float progress { get { return this.progress_; } }
        public bool complete { get { return this.manifests.Count == 0 && this.files.Count== 0; } }

        private int download_size = 0;
        private int download_speed = 0;
        private int download_file_size = 0;
        private int download_speed_size = 0;
        private float progress_ = 0;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        private bool pause = false;

        public Updater(string version) : this(version, "") { }

        public Updater(string version, string url) {
            this.url = url;
            this.cur_version = version;
            this.url = this.url.TrimEnd('/');
        }

        public bool Contains(string name) {
            if (this.hashs.Contains(name))
                return true;
            else
                return false;
        }

        // 这个接口较耗
        public bool ContainsWild(string[] wildnames) {
            foreach (var v in this.hashs) {
                for (int i = 0; i < wildnames.Length; ++i) {
                    int idx = v.IndexOf(wildnames[i], StringComparison.OrdinalIgnoreCase);
                    if (idx != -1) return true;
                }
            }
            return false;
        }

        public int Begin() {
            if (string.IsNullOrEmpty(this.url)) {
                throw new Exception("url is empty");
            }

            Request.LogAllRequests = true;
            Request.logger = logger;

            var file = new VersionFile(this, "version.txt", this.url);
            file.logger = this.logger;
            this.manifests.Enqueue(file);
            this.cdn_url = this.url;
            return 1;
        }

        public int Begin(string upversion, string cdn) {
            if (!string.IsNullOrEmpty(cdn)) this.cdn_url = cdn;
            this.cdn_url = this.cdn_url.TrimEnd('/');

            var vcur = Version.Parse(this.cur_version);
            var vnew = Version.Parse(upversion);
            if (vnew <= vcur) {
                // Don't need to update
                if (onComplete != null) onComplete(false);
                return 0;
            }

            this.logger.Info("===> update version from {0} to {1}", this.cur_version, upversion);
            if (vcur.major != vnew.major) {
                // 大版本变化，需强制更新
                if (onError != null)
                    onError(ErrorCode.INCOMPATIBLE, "");
                return -1;
            }

            if (string.IsNullOrEmpty(this.cdn_url)) {
                if (onError != null) 
                    onError(ErrorCode.CHECK_FAILED, "unknown cdn address");
                return -1;
            }

            this.total_size = 0;
            this.download_size = 0;
            this.download_speed = 0;
            this.download_file_size = 0;
            this.download_speed_size = 0;
            this.watch.Reset();

            this.files.Clear();
            this.hashs.Clear();

            Request.LogAllRequests = true;
            Request.logger = logger;

            int upvalue = vnew.code;
            int value = vcur.code;
            while (upvalue > value) {
                string field = Version.From(upvalue).ToString();
                var file = new ManifestFile(this, "version.manifest", this.cdn_url + "/" + field);
                logger.Debug("######### " + field);
                this.manifests.Enqueue(file);
                --upvalue;
            }

            this.new_version = upversion;

            return 1;
        }

        // return false when update finished, otherwise return true.
        public bool Update() {
            if (this.manifests.Count > 0) {
                if (this.current == null) {
                    this.current = this.manifests.Peek();
                    this.current.Download();
                }

                if (current.IsDone) {
                    if (current.HasError) {
                        if (onError != null) 
                            onError(ErrorCode.CHECK_FAILED, current.name);
                        return true;
                    }
                    current = null;
                    this.manifests.Dequeue();
                }

                return true;
            }

            if (this.files.Count > 0) {
                if (this.current == null) {
                    this.current = this.files.Peek();
                    this.current.Download();
                }

                if (!watch.IsRunning) watch.Start();
                this.download_size = this.download_file_size + current.Length;

                if (watch.ElapsedMilliseconds >= 50) {
                    this.download_speed = this.download_size - this.download_speed_size;
                    this.download_speed_size = this.download_size;
                    watch.Reset();
                }

                if (this.total_size > 0) {
                    float prg = (float)this.download_size / this.total_size;
                    if (prg != this.progress_) {
                        this.progress_ = prg;
                        if (this.onProgress != null) this.onProgress(this.progress_);
                    }
                }

                if (current.IsDone) {
                    watch.Stop();
                    watch.Reset();

                    if (current.HasError) {
                        if (onError != null) 
                            onError(ErrorCode.FILE_ERROR, current.name);
                        return true;
                    }

                    this.download_file_size += current.content_length;
                    current = null;
                    this.files.Pop();

                    if (this.files.Count == 0) {
                        if (this.progress_ != 1f) {
                            this.progress_ = 1;
                            if (this.onProgress != null) this.onProgress(this.progress_);
                        }

                    }
                }
                return true;
            }

            if (!string.IsNullOrEmpty(this.new_version)) {
                // complete
                string manifest_file = PathExt.MakeCachePath(MANIFEST);
                System.IO.File.WriteAllText(manifest_file, this.new_version, System.Text.Encoding.UTF8);
            }

            if (onComplete != null) onComplete(true);

            return false;
        }

        public void Continue() {
            if (current != null && current.HasError) {
                current.Download();
            }
        }

        public void End() {
            this.manifests.Clear();
            this.files.Clear();
            this.hashs.Clear();
            this.current = null;
        }

        internal void ParseVersion(string value) {
            this.new_version = value;
            logger.Info("===> new version {0}", this.new_version);
            this.Begin(this.new_version, this.cdn_url);
        }
 
        internal void ParseManifest(string value, string url) {
            Predicate<string> filter = delegate (string name) {
                if (this.hashs.Contains(name))
                    return true;
                this.hashs.Add(name);
                return false;
            };

            StringReader reader = new StringReader(value);
            string line = reader.ReadLine();
            int total_lenth = 0;
            int.TryParse(line, out total_lenth);
            while (!string.IsNullOrEmpty((line = reader.ReadLine()))) {
                string[] p = line.Split(',');
                string name = p[0];
                string path = p[1];
                int length = 0;
                int.TryParse(p[2], out length);
                string md5 = p.Length > 3 ? p[3] : "";

                if (filter != null && filter(path + name))
                    continue;

                UpdateFile file = new UpdateFile(name, url + path, PathExt.MakeCachePath(path), length, md5);
                file.logger = this.logger;
                this.files.Push(file);
                logger.Debug("** {0}", name);

                this.total_size += length;
            }
        }

        private static string[] sizetoken = { "B", "KB", "MB", "GB" };
        public static string GetSizeToken(double size) {
            int order = 0;
            while (size >= 1024.0f && order + 1 < sizetoken.Length) {
                ++order;
                size /= 1024.0f;
            }
            return string.Format("{0:0.##}{1}", size, sizetoken[order]);
        }
    }
}