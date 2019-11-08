using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassDiagramGenerator
{
    class ImageGenerator
    {
        private struct MultiList
        {
            public string Name;
            public bool IsStatic;
            public MultiList(string name, bool isStatic)
            {
                Name = name;
                IsStatic = isStatic;
            }
        }
        static FontStyle fontStyle = FontStyle.Regular;
        //Graphics flagGraphics = Graphics.FromImage(image);
        //int red = 0;
        //int white = 11;
        //while (white <= 100)
        //{
        //    flagGraphics.FillRectangle(Brushes.Red, 0, red, 200, 10);
        //    flagGraphics.FillRectangle(Brushes.White, 0, white, 200, 10);
        //    red += 20;
        //    white += 20;
        //}
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
            var methodNameLoc = 10 + textSize.Height * 2 + 10;
            foreach (var method in methods)
            {
                font = new Font(FontFamily.GenericSansSerif, 12, (method.IsStatic ? FontStyle.Underline : FontStyle.Regular));
                textSize = TextRenderer.MeasureText(method.Name, font);
                imageContent.DrawString(method.Name, font,
                    new SolidBrush(Color.Black),
                    15,
                    methodNameLoc);
                methodNameLoc += textSize.Height + 5;
            }
            var attributes = FindAttributes(tree);
            foreach (var item in attributes)
            {
                MessageBox.Show(item.Name);
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
                else if (modifiers.Contains("private"))
                {
                    modifiers = "-";
                }
                else if (modifiers.Contains("protected"))
                {
                    modifiers = "#";
                }
                //node.modifiers for accessibilty modifiers
                attributes.Add(new MultiList(modifiers + node.Declaration, isStatic));

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
                methods.Add(new MultiList(modifiers + node.Identifier.ToString() + node.ParameterList,isStatic));
                
            }


            return methods;
        }


    }
}
