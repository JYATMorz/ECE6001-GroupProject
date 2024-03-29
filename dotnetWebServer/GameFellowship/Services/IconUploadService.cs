﻿using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;

namespace GameFellowship.Services;

public class IconUploadService : IIconUploadService
{
    private const long _maxIconSize = 1024 * 1024 * 3; // 3MB
    private const string _rootPath = "wwwroot";
    private const string _saveFolderPath = "images";
    private const string _unsafePath = "UserUpload";

    public async Task<(bool, string)> IconUploadToJpg(InputFileChangeEventArgs e,
                                                      IWebHostEnvironment Environment,
                                                      string iconFolder,
                                                      string fileName,
                                                      long imageStorageSize = _maxIconSize,
                                                      int imagePixelSize = 500,
                                                      bool safeUpload = false)
    {
        if (string.IsNullOrWhiteSpace(iconFolder))
        {
            return (false, "The path of the icon folder is empty.");
        }
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return (false, "The name of the game is empty.");
        }

        if (!safeUpload)
        {
            await Task.Delay(1000);
            return (true, Path.Combine(_saveFolderPath, iconFolder, "default.jpg"));
        }

        string patern = @"[^\d\w]";
        string gameImagePath = Regex.Replace(fileName.Trim().ToLower(), patern, string.Empty) + ".jpg";
        string path = Path.Combine(Environment.ContentRootPath, _rootPath, _saveFolderPath, iconFolder, _unsafePath, gameImagePath);

        try
        {
            IBrowserFile? loadedIcon = await e.File.RequestImageFileAsync("image/jpeg", imagePixelSize, imagePixelSize);

            await using FileStream fs = new(path, FileMode.Create);
            await loadedIcon.OpenReadStream(imageStorageSize).CopyToAsync(fs);
        }
        catch (Exception error)
        {
            // TODO: Save error to log
            Console.WriteLine(error.Message);

            return (false, "Upload Failed: " + error.Message);
        }

        return (true, Path.Combine(_saveFolderPath, iconFolder, _unsafePath, gameImagePath));
    }

}
