using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Diagnostics;

namespace Buhta
{
    public static class StringBuilderExtensions
    {
        public static string AsJavaScriptStringQuoted(this StringBuilder value)
        {
            return @"'" + HttpUtility.JavaScriptStringEncode(value.ToString()) + @"'";
        }

        public static void RemoveLastChar(this StringBuilder value, int count = 1)
        {
            if (value.Length >= count)
                value.Remove(value.Length - count, count);
        }

        //public static void SaveToFile(this string value, string fileName)
        //{
        //    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        //    using (StreamWriter outfile = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
        //    {
        //        outfile.Write(value);
        //        outfile.Close();
        //    }
        //}

    }

    //public static class DrawingExtensions
    //{
    //    public static string ImageType(this Image image)
    //    {
    //        if (image.RawFormat.Equals(ImageFormat.Bmp))
    //        {
    //            return "bmp";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.MemoryBmp))
    //        {
    //            return "bmp";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Wmf))
    //        {
    //            return "emf";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Wmf))
    //        {
    //            return "wmf";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Gif))
    //        {
    //            return "gif";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Jpeg))
    //        {
    //            return "jpeg";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Png))
    //        {
    //            return "png";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Tiff))
    //        {
    //            return "tiff";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Exif))
    //        {
    //            return "exif";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Icon))
    //        {
    //            return "ico";
    //        }

    //        return "";
    //    }
    //}
    //public static class ImageExtensions
    //{
    //    public static byte[] AsByteArray(this Image imageIn)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
    //        return ms.ToArray();
    //    }

    //    public static string AsMD5Hash(this Image value)
    //    {
    //        MD5 md5 = MD5.Create();
    //        byte[] inputBytes = value.AsByteArray();
    //        byte[] hash = md5.ComputeHash(inputBytes);
    //        return Convert.ToBase64String(hash).Substring(1, 16).Replace("+", "-").Replace("/", "-");
    //    }
    //}
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception e)
        {
            var info = new StringBuilder();

            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();
            info.AppendLine(st.ToString());

            string msg = e.Message;
            e = e.InnerException;
            while (e != null)
            {
                msg += "\n" + e.Message;

                st = new StackTrace(e, true);
                frame = st.GetFrame(0);
                line = frame.GetFileLineNumber();

                e = e.InnerException;
            }



            return msg + "\n\n" + info.ToString();
        }
    }





    public static class EnumExtention
    {
        public static string ToNameString(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }
    }





    public static class StringExtention
    {
        public static string Repeat(this char chatToRepeat, int repeat)
        {

            return new string(chatToRepeat, repeat);
        }

        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }

        public static string AsMD5Hash(this string value)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
            byte[] hash = md5.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }

        //public static string AsEncrypted(this string value)
        //{
        //    return Encrypter.EncryptString(value);
        //}

        //public static string AsDecrypted(this string value)
        //{
        //    return Encrypter.DecryptString(value);
        //}



    }

    //public static class DateTimeExtensions
    //{


    //    public static DateTime WithOutTime(this DateTime date)
    //    {
    //        return new DateTime(date.Year, date.Month, date.Day);

    //    }
    //    //public static string ToSQLiteWithOutTime(this DateTime date)
    //    //{
    //    //    return new DateTime(date.Year, date.Month, date.Day).ToString("yyyy-MM-dd");

    //    //}
    //    public static string AsSQL(this DateTime date)
    //    {
    //        return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
    //    }




    //}

    //public static class DrawingExtensions
    //{
    //    public static string ImageType(this Image image)
    //    {
    //        if (image.RawFormat.Equals(ImageFormat.Bmp))
    //        {
    //            return "bmp";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.MemoryBmp))
    //        {
    //            return "bmp";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Wmf))
    //        {
    //            return "emf";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Wmf))
    //        {
    //            return "wmf";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Gif))
    //        {
    //            return "gif";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Jpeg))
    //        {
    //            return "jpeg";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Png))
    //        {
    //            return "png";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Tiff))
    //        {
    //            return "tiff";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Exif))
    //        {
    //            return "exif";
    //        }
    //        else if (image.RawFormat.Equals(ImageFormat.Icon))
    //        {
    //            return "ico";
    //        }

    //        return "";
    //    }
    //}
    //public static class ImageExtensions
    //{
    //    public static byte[] AsByteArray(this Image imageIn)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
    //        return ms.ToArray();
    //    }

    //    public static string AsMD5Hash(this Image value)
    //    {
    //        MD5 md5 = MD5.Create();
    //        byte[] inputBytes = value.AsByteArray();
    //        byte[] hash = md5.ComputeHash(inputBytes);
    //        return Convert.ToBase64String(hash).Substring(1, 16).Replace("+", "-").Replace("/", "-");
    //    }
    //}


    public static class StringExtensions
    {
        public static string RepairWrongRussianKeyboard(this string value)
        {
            value = value.Replace('q', 'й').Replace('Q', 'Й');
            value = value.Replace('w', 'ц').Replace('W', 'Ц');
            value = value.Replace('e', 'у').Replace('E', 'У');
            value = value.Replace('r', 'к').Replace('R', 'К');
            value = value.Replace('t', 'е').Replace('T', 'Е');
            value = value.Replace('y', 'н').Replace('Y', 'Н');
            value = value.Replace('u', 'г').Replace('U', 'Г');
            value = value.Replace('i', 'ш').Replace('I', 'Ш');
            value = value.Replace('o', 'щ').Replace('O', 'Щ');
            value = value.Replace('p', 'з').Replace('P', 'З');
            value = value.Replace('[', 'х').Replace('[', 'Х').Replace('{', 'Х');
            value = value.Replace(']', 'ъ').Replace(']', 'Ъ').Replace('}', 'Ъ');
            value = value.Replace('a', 'ф').Replace('A', 'Ф');
            value = value.Replace('s', 'ы').Replace('S', 'Ы');
            value = value.Replace('d', 'в').Replace('D', 'В');
            value = value.Replace('f', 'а').Replace('F', 'А');
            value = value.Replace('g', 'п').Replace('G', 'П');
            value = value.Replace('h', 'р').Replace('H', 'Р');
            value = value.Replace('j', 'о').Replace('J', 'О');
            value = value.Replace('k', 'л').Replace('K', 'Л');
            value = value.Replace('l', 'д').Replace('L', 'Д');
            value = value.Replace(';', 'ж').Replace(':', 'Ж').Replace(';', 'Ж');
            value = value.Replace("'", "э").Replace('"', 'Э').Replace("'", "Э");
            value = value.Replace('z', 'я').Replace('Z', 'Я');
            value = value.Replace('x', 'ч').Replace('X', 'Ч');
            value = value.Replace('c', 'с').Replace('C', 'С');
            value = value.Replace('v', 'м').Replace('V', 'М');
            value = value.Replace('b', 'и').Replace('B', 'И');
            value = value.Replace('n', 'т').Replace('N', 'Т');
            value = value.Replace('m', 'ь').Replace('M', 'Ь');
            value = value.Replace(',', 'б').Replace('<', 'Б').Replace(',', 'Б');
            value = value.Replace('.', 'ю').Replace('>', 'Ю').Replace('.', 'Ю');
            value = value.Replace('/', '.').Replace('?', ',');
            value = value.Replace('`', 'ё').Replace('~', 'Ё').Replace('`', 'Ё');

            return value;
        }

        // перевод русских названий полей в англ. транскрипцию, без этого глючит Ext.XTemplate
        static Dictionary<string, string> transMap;
        public static string TranslateRusToEng(this string str)
        {
            if (transMap == null)
            {
                transMap = new Dictionary<string, string>();

                transMap.Add(".", "_");
                transMap.Add("А", "A");
                transMap.Add("Б", "B");
                transMap.Add("В", "V");
                transMap.Add("Г", "G");
                transMap.Add("Д", "D");
                transMap.Add("Е", "E");
                transMap.Add("Ж", "G");
                transMap.Add("З", "Z");
                transMap.Add("И", "I");
                transMap.Add("Й", "J");
                transMap.Add("К", "K");
                transMap.Add("Л", "L");
                transMap.Add("М", "M");
                transMap.Add("Н", "N");
                transMap.Add("О", "O");
                transMap.Add("П", "P");
                transMap.Add("Р", "R");
                transMap.Add("С", "S");
                transMap.Add("Т", "T");
                transMap.Add("У", "U");
                transMap.Add("Ф", "F");
                transMap.Add("Х", "H");
                transMap.Add("Ц", "C");
                transMap.Add("Ч", "CH");
                transMap.Add("Ш", "SH");
                transMap.Add("Щ", "SH_");
                transMap.Add("Ъ", "_");
                transMap.Add("Ы", "IY");
                transMap.Add("Ь", "_");
                transMap.Add("Э", "E");
                transMap.Add("Ю", "JY");
                transMap.Add("Я", "JA");

                transMap.Add("а", "a");
                transMap.Add("б", "b");
                transMap.Add("в", "v");
                transMap.Add("г", "g");
                transMap.Add("д", "d");
                transMap.Add("е", "e");
                transMap.Add("ж", "g");
                transMap.Add("з", "z");
                transMap.Add("и", "i");
                transMap.Add("й", "j");
                transMap.Add("к", "k");
                transMap.Add("л", "l");
                transMap.Add("м", "m");
                transMap.Add("н", "n");
                transMap.Add("о", "o");
                transMap.Add("п", "p");
                transMap.Add("р", "r");
                transMap.Add("с", "s");
                transMap.Add("т", "t");
                transMap.Add("у", "u");
                transMap.Add("ф", "f");
                transMap.Add("х", "h");
                transMap.Add("ц", "c");
                transMap.Add("ч", "ch");
                transMap.Add("ш", "sh");
                transMap.Add("щ", "sh_");
                transMap.Add("ъ", "_");
                transMap.Add("ы", "iy");
                transMap.Add("ь", "_");
                transMap.Add("э", "e");
                transMap.Add("ю", "ju");
                transMap.Add("я", "ja");
            }
            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (transMap.Keys.Contains(str[i].ToString()))
                    sb.Append(transMap[str[i].ToString()]);
                else
                    sb.Append(str[i].ToString());
            }
            return sb.ToString();
        }

        public static string TranslateToCorrectFileName(this string Str)
        {
            var sb = new StringBuilder();
            var str = Str.Trim();
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetterOrDigit(str[i]))
                    sb.Append(str[i].ToString());
                else
                    sb.Append("_");
            }
            return sb.ToString().Trim();
        }

        //    public static string InsertParams(this string str, string param1, string param2 = null, string param3 = null, string param4 = null, string param5 = null)
        //    {
        //        str = str.Replace("%1", param1);
        //        if (param2 != null) str = str.Replace("%2", param2);
        //        if (param3 != null) str = str.Replace("%3", param3);
        //        if (param4 != null) str = str.Replace("%4", param4);
        //        if (param5 != null) str = str.Replace("%5", param5);
        //        return str;
        //    }

        //    public static bool IsNullOrWhiteSpace(this string value)
        //    {
        //        if (value == null) return true;
        //        return string.IsNullOrEmpty(value.Trim());
        //    }

        public static void SaveToFile(this string value, string fileName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using (StreamWriter outfile = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
            {
                outfile.Write(value);
                outfile.Close();
            }
        }


        //    public static string WithOutLastChar(this string value)
        //    {
        //        if (value == null) return null;
        //        if (value.Length == 0) return value;
        //        return value.Substring(0, value.Length - 1);
        //    }

        //    public static string WithLastSemicolon(this string value)
        //    {
        //        if (value == null) return null;
        //        if (value.Length == 0) return value;
        //        if (value.Last() == ';')
        //            return value;
        //        else
        //            return value + ";";
        //    }

        //    public static string WithLastSpace(this string value)
        //    {
        //        if (value == null) return null;
        //        if (value.Length == 0) return value;
        //        if (value.Last() == ' ')
        //            return value;
        //        else
        //            return value + " ";
        //    }


        public static string AsRemoveLastChar(this string value, int count = 1)
        {
            if (value == null || value.Length < count) return value;
            return value.Remove(value.Length - count, count);
        }

        public static string AsValidCSharpIdentifier(this string value)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            string ret = regex.Replace(value, "_");
            if (!char.IsLetter(ret, 0) && ret[0] != '_')
                ret = string.Concat("_", ret);
            return ret;
        }

        public static string AsSQL(this string value)
        {
            if (value == null)
                return "NULL";
            else
                return "'" + value.Replace("'", "''") + "'";
        }

        public static string AsLIKE(this string value)
        {
            if (value == null) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        //    public static string AsMD5Hash(this string value)
        //    {
        //        MD5 md5 = MD5.Create();
        //        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
        //        byte[] hash = md5.ComputeHash(inputBytes);
        //        return Convert.ToBase64String(hash).Substring(1, 12).Replace("+", "-");
        //    }

        public static string AsJavaScript(this string value)
        {
            return HttpUtility.JavaScriptStringEncode(value, true);
        }

        public static string AsJavaScriptStringQuoted(this string value)
        {
            return HttpUtility.JavaScriptStringEncode(value, true);
        }

        public static string AsJavaScriptString(this string value)
        {
            return HttpUtility.JavaScriptStringEncode(value);
        }

        public static string WithOutLastChar(this string value)
        {
            if (value == null) return null;
            if (value.Length == 0) return value;
            return value.Substring(0, value.Length - 1);
        }

        public static string WithLastSemicolon(this string value)
        {
            if (value == null) return null;
            if (value.Length == 0) return value;
            if (value.Last() == ';')
                return value;
            else
                return value + ";";
        }

        public static string WithLastSpace(this string value)
        {
            if (value == null) return null;
            if (value.Length == 0) return value;
            if (value.Last() == ' ')
                return value;
            else
                return value + " ";
        }
        public static MemoryStream AsMemoryStream(this string value)
        {
            if (value == null) return null;
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(value);
            sw.Flush();
            return ms;
        }

        public static string AsHtml(this string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        public static string AsHtmlAttribute(this string value)
        {
            return HttpUtility.HtmlAttributeEncode(value);
        }

        public static string AsHtmlAttributeQuoted(this string value)
        {
            return @"""" + HttpUtility.HtmlAttributeEncode(value) + @"""";
        }

        public static string AsSqlStringQuoted(this string value)
        {
            return @"'" + value.Replace("'", "''") + @"'";
        }


        public static string Repeat(this char chatToRepeat, int repeat)
        {

            return new string(chatToRepeat, repeat);
        }

        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }

        public static string CreateFromFile(this string fake, string fileName)
        {
            var stream = new MemoryStream();
            stream.LoadFromFile(fileName);
            return stream.AsString();
        }

    }




    public static class GuidExtensions
    {
        public static Guid Reverse(this Guid value)
        {
            return new Guid(value.ToByteArray().Reverse().ToArray());
        }

        public static string AsSQL(this Guid value)
        {
            return "'" + value + "'";
        }

        public static string AsJavaScript(this Guid value)
        {
            return "'" + value + "'";
        }

        public static string AsSQL(this Guid? value)
        {
            if (value == null)
                return "NULL";
            else
                return "'" + value + "'";
        }
    }

    public static class DateTimeExtensions
    {
        public static string AsSQL_DateAndTime(this DateTime value)
        {
            return "'" + ((DateTime)value).ToString("yyyyMMdd HH:mm:ss.fff") + "'";
        }
        public static string AsSQL_OnlyDate(this DateTime value)
        {
            return "'" + ((DateTime)value).ToString("yyyyMMdd") + "'";
        }
        public static string AsSQL_OnlyTime(this DateTime value)
        {
            return "'" + ((DateTime)value).ToString("hh:mm:ss") + "'";
        }

        public static DateTime WithOutTime(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);

        }

    }

    public static class DoubleExtensions
    {
        public static string AsSQL(this Double value)
        {
            return value.ToString("0.###############");
        }
        public static string AsJavaScript(this Double value)
        {
            return value.ToString("0.###############");
        }
    }

    public static class ObjectExtensions
    {

        public static object GetPropertyValue(this Object obj, string propName)
        {
            var names = propName.Split('.');
            for (int i = 0; i < names.Length; i++)
            {
                Type _type = obj.GetType();
                PropertyInfo _prop = _type.GetProperty(names[i]);
                if (_prop == null)
                {
                    if (names.Length > 0)
                        throw new Exception("объект '" + obj.GetType().FullName + "'." + nameof(GetPropertyValue) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
                    else
                        throw new Exception("объект '" + obj.GetType().FullName + "'." + nameof(GetPropertyValue) + ": не найдено свойство '" + propName + "'");
                }
                obj = _prop.GetValue(obj);
                if (obj == null)
                    return null;
                if (i == names.Length - 1)
                    return obj;
            }
            return null;
        }

        public static string AsSQL(this Object value)
        {
            if (value == null || value is DBNull)
                return "NULL";
            if (value is string)
                return (value as string).AsSQL();
            if (value is StringBuilder)
                return (value as StringBuilder).ToString().AsSQL();
            if (value is int)
                return value.ToString();
            if (value is Double)
                return ((Double)value).AsSQL();
            if (value is DateTime)
                return ((DateTime)value).AsSQL_DateAndTime();
            if (value is Guid)
                return ((Guid)value).AsSQL();
            //if (value is mixEnumElement)
            //    return ((mixEnumElement)value).ID.AsSQL();
            if (value is float)
                return ((Double)value).AsSQL();
            if (value is bool)
                return ((bool)value) ? "1" : "0";

            throw new Exception("Object.AsSQL(): неизвестный класс '" + value.GetType().FullName + "'");
        }

        public static string AsJavaScript(this Object value)
        {
            if (value == null || value is DBNull)
                return "null";
            if (value is string)
                return (value as string).AsJavaScript();
            if (value is StringBuilder)
                return (value as StringBuilder).ToString().AsJavaScript();
            if (value is int)
                return value.ToString();
            if (value is Double)
                return ((Double)value).AsJavaScript();
            //if (value is DateTime)
            //  return ((DateTime)value).AsSQL_DateAndTime();
            if (value is Guid)
                return ((Guid)value).AsJavaScript();
            if (value is float)
                return ((Double)value).AsJavaScript();
            if (value is bool)
                return ((bool)value) ? "true" : "false";
            if (value is JsBaseObject)
                return (value as JsBaseObject).ToJson();

            throw new Exception("Object." + nameof(AsJavaScript) + "(): неизвестный класс '" + value.GetType().FullName + "'");
        }

        public static byte[] AsSerializedImage(this Object value)
        {
            var fs1 = new MemoryStream();
            BinaryFormatter bin = new BinaryFormatter();
            bin.Serialize(fs1, value);
            return fs1.ToArray();
        }

        public static string AsSerializedImageHash(this Object value)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            return Convert.ToBase64String(sha1.ComputeHash(value.AsSerializedImage()));
        }

        public static string ToStringOrNull(this Object value)
        {
            if (value == null || value == System.DBNull.Value)
                return null;
            else
                return value.ToString();
        }

        public static Guid? ToGuidOrNull(this Object value)
        {
            if (value == null || value == System.DBNull.Value)
                return null;
            else
                return (Guid)value;
        }

        public static object EvalPropertyValue(this Object obj, string propName)
        {
            var names = propName.Split('.');
            for (int i = 0; i < names.Length; i++)
            {
                Type _type = obj.GetType();
                PropertyInfo _prop = _type.GetProperty(names[i]);
                if (_prop == null)
                    throw new Exception("model." + nameof(EvalPropertyValue) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
                obj = _prop.GetValue(obj);
                if (obj == null)
                    return null;
                if (i == names.Length - 1)
                    return obj;
            }
            return null;
        }
    }



    public static class MemoryStreamExtensions
    {
        //public static MemoryStream GetZiped(this MemoryStream stream)
        //{

        //    MemoryStream outputMemStream = new MemoryStream();
        //    ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

        //    zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

        //    ZipEntry newEntry = new ZipEntry("quotes");
        //    newEntry.DateTime = DateTime.Now;

        //    zipStream.PutNextEntry(newEntry);

        //    stream.Position = 0;
        //    StreamUtils.Copy(stream, zipStream, new byte[4096]);
        //    zipStream.CloseEntry();

        //    zipStream.IsStreamOwner = false;
        //    zipStream.Close();

        //    outputMemStream.Position = 0;
        //    return outputMemStream;
        //}

        //public static void LoadFromZipedByteArray(this MemoryStream stream, Byte[] array)
        //{
        //    stream.Position = 0;
        //    ZipInputStream zipInputStream = new ZipInputStream(new MemoryStream(array));
        //    ZipEntry zipEntry = zipInputStream.GetNextEntry();
        //    byte[] buffer = new byte[4096];		// 4K is optimum
        //    StreamUtils.Copy(zipInputStream, stream, buffer);
        //    zipEntry = zipInputStream.GetNextEntry();
        //    stream.Position = 0;

        //}

        public static void LoadFromFile(this MemoryStream stream, string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            stream.Position = 0;
            file.CopyTo(stream);
            stream.Position = 0;
        }

        public static void SaveToFile(this MemoryStream stream, string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create))
            {
                stream.CopyTo(file);
                stream.Position = 0;
            }
        }

        public static string AsString(this MemoryStream stream)
        {
            stream.Position = 0;
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

    }
}
