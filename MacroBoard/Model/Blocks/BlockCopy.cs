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
    public class BlockCopy : Block
    {
        public String source { get; set; } = "";
        public String destination { get; set; } = "";
        
        public BlockCopy(String source, String destination)
        {

            base.Name = "Copy Directory";
            base.info = "Copies all the elements of a directory to another directory.";
            base.LogoUrl = "/Resources/Logo_Blocks/Logo_BlockCopyFolder.png";

            this.source = source;
            this.destination = destination;
            base.category = Categories.Files;

        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);
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

            if (attr.HasFlag(FileAttributes.Directory)) {
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
