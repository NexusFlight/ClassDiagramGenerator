using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassDiagramGenerator
{
    class ImageGenerator
    {
        

        static FontStyle fontStyle = FontStyle.Regular;

        public static Bitmap GenerateImage(string fileInput, string fileOutput)
        {

            var tree = CSharpSyntaxTree.ParseText(FileHandler.OpenFile(fileInput));
            
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            Bitmap image = new Bitmap(resolution.Width, resolution.Height);
            Graphics imageContent = Graphics.FromImage(image);

            var className = GetClassName(tree);
            var methods = FindMethods(tree);
            var recHeight = 100 * methods.Capacity;
            var pen = new Pen(Color.Black);
            var font = new Font(FontFamily.GenericSansSerif, 15, fontStyle);
            imageContent.TextRenderingHint = TextRenderingHint.AntiAlias;
            var textSize = TextRenderer.MeasureText(className, font);
            //overall rectangle
            imageContent.DrawRectangle(pen, 10, 10, 500, recHeight);
            //classname
            imageContent.DrawString(className,font,
                new SolidBrush(Color.Black),
                250-(textSize.Width/2),
                10 + textSize.Height/2);
            //seperator line
            imageContent.DrawLine(pen, 10, 10 + textSize.Height*2, 510, 10 + textSize.Height * 2);

            var attributes = FindAttributes(tree);

            var drawLoc = 10 + textSize.Height * 2 + 10;
            foreach (var attrib in attributes)
            {
                font = new Font(FontFamily.GenericSansSerif, 12, (attrib.IsStatic ? FontStyle.Underline : FontStyle.Regular));
                textSize = TextRenderer.MeasureText(attrib.Name, font);
                imageContent.DrawString(attrib.Name, font,
                    new SolidBrush(Color.Black),
                    15,
                    drawLoc);
                drawLoc += textSize.Height + 5;
            }
            //seperator line
            imageContent.DrawLine(pen, 10, drawLoc, 510, drawLoc);
            foreach (var method in methods)
            {
                font = new Font(FontFamily.GenericSansSerif, 12, (method.IsStatic ? FontStyle.Underline : FontStyle.Regular));
                textSize = TextRenderer.MeasureText(method.Name, font);
                imageContent.DrawString(method.Name, font,
                    new SolidBrush(Color.Black),
                    15,
                    drawLoc);
                drawLoc += textSize.Height + 5;
            }
            


            //FileHandler.SaveBitmap(image, fileOutput);
            return image;
        }

        private static string GetClassName(SyntaxTree tree)
        {
            var root = tree.GetRoot();
            var className = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First().Identifier.ToString();
            return className;
        }

        private static List<MultiList> FindAttributes(SyntaxTree tree)
        {
            var root = tree.GetRoot();
            var attributes = new List<MultiList>();
            foreach (var node in root.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                var modifiers = node.Modifiers.ToString();
                var isStatic = false;
                if (modifiers.Contains(" "))
                {
                    var spaceLoc = modifiers.IndexOf(" ");
                    modifiers = modifiers.Remove(spaceLoc, modifiers.Length - spaceLoc);
                    isStatic = true;
                }else if (modifiers.ToString() == "static")
                {
                    isStatic = true;
                    modifiers = "";
                }
                if (modifiers.Contains("public"))
                {
                    modifiers = "+";
                }
                else if (modifiers.Contains("protected"))
                {
                    modifiers = "#";
                }
                else
                {
                    modifiers = "-";
                }
                //node.modifiers for accessibilty modifiers
                attributes.Add(new MultiList(modifiers + OrderAttributes(node.Declaration.ToString()), isStatic));

            }


            return attributes;
        }


        private static List<MultiList> FindMethods(SyntaxTree tree)
        {
            var root = tree.GetRoot();
            var methods = new List<MultiList>();
            foreach (var node in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                var modifiers = node.Modifiers.ToString();
                var isStatic = false;
                if(modifiers.Contains(" "))
                {
                    var spaceLoc = modifiers.IndexOf(" ");
                    modifiers = modifiers.Remove(spaceLoc, modifiers.Length - spaceLoc);
                    isStatic = true;
                }
                if (modifiers.Contains("public"))
                {
                    modifiers = "+";
                }
                else if (modifiers.Contains("private"))
                {
                    modifiers = "-";
                }
                else if (modifiers.Contains("protected"))
                {
                    modifiers = "#";
                }
                //node.modifiers for accessibilty modifiers
                methods.Add(new MultiList(modifiers + node.Identifier.ToString() + OrderParameters(node.ParameterList.ToString()),isStatic));
                
            }


            return methods;
        }







        private static string OrderParameters(string parameterList)
        {
            string pattern = @"(?:([a-zA-Z0-9_]*) ([a-zA-Z0-9_]*))";

            parameterList = parameterList.Replace(", ",",");
            parameterList = parameterList.Replace(" ,", ",");

            parameterList = parameterList.Replace("( ", "(");
            parameterList = parameterList.Replace(" )", ")");



            var matches = Regex.Matches(parameterList, pattern, RegexOptions.IgnoreCase);

            var sb = new StringBuilder();

            sb.Append("(");
            foreach (Match match in matches)
            {
                sb.Append(match.Groups[2] + ":" + match.Groups[1]);
                if(match != matches[matches.Count-1])
                {
                    sb.Append(",");
                }
            }
            sb.Append(")");

            return sb.ToString();
        }

        private static string OrderAttributes(string attributes)
        {
            string pattern = @"(?:([a-zA-Z0-9_]*) ([a-zA-Z0-9_]*)([=a-zA-Z0-9_. ()]*))";

            attributes = attributes.Replace(", ", ",");
            attributes = attributes.Replace(" ,", ",");

            attributes = attributes.Replace("( ", "(");
            attributes = attributes.Replace(" )", ")");



            var matches = Regex.Matches(attributes, pattern, RegexOptions.IgnoreCase);

            var sb = new StringBuilder();

            foreach (Match match in matches)
            {
                sb.Append(match.Groups[2] + ":" + match.Groups[1] + match.Groups[3]);
            }

            return sb.ToString();
        }
    }
}
