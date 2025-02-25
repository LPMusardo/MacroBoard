﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MacroBoard.Utils;

namespace MacroBoard
{
    [Serializable]
    public class BlockCopyFile : Block
    {
        public String source { get; set; } = "";
        public String destination { get; set; } = "";

        public BlockCopyFile(String source, String destination)
        {
            base.Name = "Copy File";
            base.info = "Copies file to another directory.";
            base.category = Categories.Files;
            this.source = source;
            this.LogoUrl = "/Resources/Logo_Blocks/Logo_BlockCopy.png";
            this.destination = destination;
        }

        static void CopyDirectory(string source, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(source);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }



        public override void Execute()
        {
            FileAttributes attr = File.GetAttributes(source);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                CopyDirectory(source, destination, true);
            }

            else
            {
                FileInfo file = new FileInfo(source);
                file.CopyTo(concatPathWithFileName(destination, Path.GetFileName(source)));
            }

        }

        public override void Accept(IBlockVisitor visitor)
        {
            visitor.Visit(this);
        }


    }
}
