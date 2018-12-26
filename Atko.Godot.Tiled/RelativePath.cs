using System;
using System.Linq;
using Godot;

namespace Atko.Godot.Tiled.RelativePathing {
    public static class RelativePath {
        public static string GetPath(string basePath, string relativePath) {
            var fp = basePath.Contains("res://") ? basePath.Split("res://")[0] : basePath;
            
            var backtrackCount = relativePath
                .Split("/")
                .Count(s => s == "..");
            
            if (backtrackCount > fp.Count(s => s == '/')) {
                throw new Exception("Error: Attempting to access a file outside of the resource directory." +
                                    "BasePath: " + basePath +
                                    "RelativePath: " + relativePath);
            }

            var finalPath = "";
            if (backtrackCount > 0) {
                var cutPathBaseArray = fp
                    .Split("/")
                    .Take(fp.Split("/").Length - backtrackCount)
                    .ToArray();

                var tsxPathArray = relativePath
                    .Split("/")
                    .Where(s => s != "..");
                
                finalPath = "res://" + string.Join("/", cutPathBaseArray) + "/" + string.Join("/", tsxPathArray);
            } else if (relativePath.Contains("./")) {
                finalPath = fp + relativePath.Split("./")[1];
            } else {
                finalPath = fp + "/" + relativePath;
            }
            
            return finalPath;
        }
    }
}