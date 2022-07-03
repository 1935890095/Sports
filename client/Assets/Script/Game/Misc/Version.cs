namespace XFX.Misc {
    using System;

    public struct Version {
        // private string _version = "0.0.0";
        private readonly int _code;
        private readonly int _major;
        private readonly int _minor;
        private readonly int _revis;

        public int major { get { return this._major; } }
        public int minor { get { return this._minor; } }
        public int revision { get { return this._revis; } }
        public int code { get { return this._code; } }

        private Version(int major, int minor, int revision) {
            this._major = major;
            this._minor = minor;
            this._revis = revision;
            this._code = this._major * 10000 + this._minor * 10 + this._revis;
        }

        public static Version From(int code) {
            int major = code / 10000;
            int minor = (code % 10000) / 10;
            int revis = code % 10;
            return new Version(major, minor, revis);
        }

        public static Version From(int major, int minor, int revision) {
            return new Version(major, minor, revision);
        }

        public static Version Parse(string s) { 
            string[] ss = s.Split('.');
            int major = int.Parse(ss[0]);
            int minor = int.Parse(ss[1]);
            int revis = int.Parse(ss[2]);
            return new Version(major, minor, revis);
        }

        public static bool TryParse(string s, out Version version)  {
            version = new Version(0, 0, 0);
            string[] ss = s.Split('.');
            if (ss.Length < 3) { return false; }
            int major, minor, revision;
            if (!int.TryParse(ss[0], out major)) { return false; }
            if (!int.TryParse(ss[1], out minor)) { return false; }
            if (!int.TryParse(ss[2], out revision)) { return false; }
            version = new Version(major, minor, revision);
            return true;
        }

        public override string ToString() {
            return string.Format("{0}.{1}.{2}", this.major, this.minor, this.revision);
        }

        public static bool operator ==(Version v1, Version v2) {
            return v1._major == v2._major && v1._minor == v2._minor && v1._revis == v2._revis;
        }
        public static bool operator !=(Version v1, Version v2) {
            return (v1 == v2) ? false : true;
        }

        // override object.Equals
        public override bool Equals(object obj) {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            var v1 = this;
            var v2 = (Version)obj;
            if ((System.Object)v2 == null) {
                return false;
            }
            return v1._major == v2._major && v1._minor == v2._minor && v1._revis == v2._revis;
        }
        
        // override object.GetHashCode
        public override int GetHashCode() {
            return this._major ^ this._minor ^ this._revis;
        }

        public static bool operator <=(Version v1, Version v2) {
            if (v1 == v2) return true;
            if (v1 < v2) return true;
            return false;
        }

        public static bool operator >=(Version v1, Version v2) {
            if (v1 == v2) return true;
            if (v1 > v2) return true;
            return false;
        }

        public static bool operator <(Version v1, Version v2) {
            if (v1._major < v2._major) return true;
            if (v1._major > v2._major) return false;
            // v1._major == v2._major
            if (v1._minor < v2._minor) return true;
            if (v1._minor > v2._minor) return false;
            // v1._minor = v2._minor
            if (v1._revis < v2._revis) return true;
            // v1._revis >= v2._revis
            return false;
        }

        public static bool operator >(Version v1, Version v2) {
            if (v1._major > v2._major) return true;
            if (v1._major < v2._major) return false;
            // v1._major == v2._major
            if (v1._minor > v2._minor) return true;
            if (v1._minor < v2._minor) return false;
            // v1._minor = v2._minor
            if (v1._revis > v2._revis) return true;
            // v1._revis <= v2._revis
            return false;
        }
    }
}