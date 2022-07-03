using System.IO;
using System.Security.Cryptography;

namespace XFX.Misc {
    public static class Crypto {
        private static readonly byte[] s_desKey = new byte[] { 35, 224, 77, 193, 24, 90, 85, 104 };
        private static readonly byte[] s_desIV = new byte[] { 22, 32, 81, 159, 78, 1, 119, 51 };

        private static readonly string s_rsaPrivateKey = @"<RSAKeyValue><Modulus>2PgHUWH1ZZpncDkkiNMeDekUd49jrsg9WZUMeBIg+aVIXrhHi64uC0Tg4UJMmZEvdATU
bW9wNzcqr6mQsACi/s3x0gzAIPuQsk2+mCB4TzoJXIpt1FDCXX81nPqkUdcmiiojk/1dgwHQymbUkxXR7Bnnvg7a8TdmDbxYdHcbX2y6cpCl1wRNMNn/h1jyPTUND2Z9kUcAkYKPwhuUg
j11qAnpoCcQwIpwHnw28wSkTCNYofkKpTKQa1DIcaY0pkBxtTO02aXTMRFvk7syKrRIG2m9VAKDYifVkaxPFjnobmjnuskTZRkIBsovjsUyYdTnWqoGD3l8YLewqB2Sc49mFQ==</Modu
lus><Exponent>AQAB</Exponent><P>6PZgaYj0QvZQCKAM58aVtFchfPt0+ZNl++eTZpxezwYLBkLbNsRSwHYCUBDsj+ZJMj+Kyqhd86h7aKZ1oTPm+mw6FsQaXyDq14n7xvi8fcOgg
F38MaQ+5pjn/dhPsBJnm10baUPeMAIEeu4v1IFPbTM7hOBDtW3+Pacs04/hQZs=</P><Q>7mzDUlVxsEabH5CVX1lxRUpoRjrPhslX/N/Jlk80CYl+3cXd+TT6O3XYPdu/KpIFJ8J4utu
+8xDyfP8hxl9DanXwCuMh1oyDnR6aFRZKjtsKpseinPECgn9Tjg7cwqPEv0P3rUyClvx39okO3rA/g+RlUuovNBkicus2WQttig8=</Q><DP>A/0D1Yq0s9jHsf4benRZ58Z+GkerzwIJ
AvEKAv22WdD+q1LFsgBWclkS3RANHMGSizuvQpzzTtKZmkcPSH08/RiRPgaJdQt5Octw9g0YHQcWXlw9jxQCx6vifOQRKTCHeY90gq7YxPWj4f7l7rtwAqyX3kSApECH08Ji8BRMhPU=<
/DP><DQ>Onhhzic2Ikyubic++ossxBljSRXTHvLxqQoYncv+rvgRLMIBlcPwOfTEeycQ0pfMu9ttUCXjxig5z32iAswJ9GnH3GOnQL5k6HtAvjshPbdp/UyoDGTugKJJE2WBrhoLffxlG
RLSU8TGhqdGBkaizO2OC69ytAwV7qjQQzYSPvE=</DQ><InverseQ>mcvLmHL6NAriQRC/pCiMwZTZtFuawq96mg8QGE182uekJbD2robc3tEnC915LeIViFFlITIIpGM7L/rEoAd8IEF
fzO7BbJoSNfpgCC15CtmN6NydMrrfmWeWK5ZKnsvV8zwVApAw0euV0P0Vp+Qqun80VoDXeY2R2D2jyd0wu8s=</InverseQ><D>t4BukAINjjUcrullYONNU+6BhO0dHcH1QUKyHQsYII
wNvS6WNCjIgHdsypdMOcV1IObE0xmhCtH5pEABQxoGwQJlN4XZKQwawPLH89niZv9q032wjhUPjgv8yBujxiP1UH17QbrTz+1FLSnzN8MInGr8eN4PhluYUJ2vIjFqUTURfQZcjA1BfvH
N5mTwK+nAPPamFjrzO63ZE5EGnEMIQqRkFkK0DU6IbBoV3mxstuA5ygfY5LZxoyWFM5iBEVdg2UwL1mNE+QRqu14EibvlyAdY9TsWuwVXUQ5xxeXYXk23+LrWbwK23dMCptr1z1ocgW3O
WErLAISZaccQQ+m/sQ==</D></RSAKeyValue>";

        private static readonly string s_rsaPublicKey = @"<RSAKeyValue><Modulus>p33C57IHRmcD6t7Bt4SpOEj3DA0QUO/M5xfngDe83YfIDbdqdjCCNXh98H/YxCP+SPxYz
z+0jlS5Hmg1X5CI32JfMpze8dMuR9NDaTmlHv+ohXShUTG3XIoYn/wHHNmBYy07UDL5CCLRz8NTdOguwhH0uYtuvTN/RZ/neI+J50JGOw+iaAcX7lM9vq851ZMR9XA++PfeE8qZS5MNGL
7ti+fDdycb5EmkFq3JwmLHUoG6WniErt8UBZHRduk/DDOSw45hy9Dpz8DiywBpXx9fTQs7iMyMep7QV7AMZJCRDwswa51lGliIwxSWGoXhiKskqWjrHxMfQq+AhLjefpjlfQ==</Modul
us><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static readonly RSA s_rsaEncryptService;
        private static readonly RSA s_rsaDecryptService;

        static Crypto() {
            s_rsaEncryptService = RSA.Create();
            s_rsaEncryptService.FromXmlString(s_rsaPublicKey);

            s_rsaDecryptService = RSA.Create();
            s_rsaDecryptService.FromXmlString(s_rsaPrivateKey);
        }

        public static byte[] DesEncrypt(byte[] datas) {
            using(DES des = DES.Create()) {
                using(MemoryStream ms = new MemoryStream()) {
                    using(CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(s_desKey, s_desIV), CryptoStreamMode.Write)) {
                        encStream.Write(datas, 0, datas.Length);
                        encStream.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        public static byte[] DesDecrypt(byte[] datas) {
            using(DES des = DES.Create()) {
                using(MemoryStream ms = new MemoryStream()) {
                    using(CryptoStream encStream = new CryptoStream(ms, des.CreateDecryptor(s_desKey, s_desIV), CryptoStreamMode.Write)) {
                        encStream.Write(datas, 0, datas.Length);
                        encStream.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        public static byte[] RSAEncrypt(byte[] datas) {
            return s_rsaEncryptService.Encrypt(datas, RSAEncryptionPadding.Pkcs1);
        }

        public static byte[] RSADecrypt(byte[] datas) {
            return s_rsaDecryptService.Decrypt(datas, RSAEncryptionPadding.Pkcs1);
        }
    }
}