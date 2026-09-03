using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameHub.Forms;
using GameHub.Models;

namespace GameHub.Services
{
    /// <summary>
    /// Maps a database GameID to local high-resolution artwork.
    /// No database image column is required.
    /// </summary>
    internal static class GameAssetService
    {
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, Image> Generated =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        public static string AssetsRoot
        {
            get { return Path.Combine(Application.StartupPath, "Assets"); }
        }

        public static Image GetCover(Game game, int fallbackWidth, int fallbackHeight)
        {
            Image image = TryGameAsset(game == null ? 0 : game.GameID,
                "cover.jpg", "cover.png", "poster.jpg", "poster.png");
            if (image != null) return image;
            return GeneratedFallback("cover", game, Math.Max(600, fallbackWidth), Math.Max(900, fallbackHeight));
        }

        public static Image GetBanner(Game game, int fallbackWidth, int fallbackHeight)
        {
            Image image = TryGameAsset(game == null ? 0 : game.GameID,
                "banner.jpg", "banner.png", "hero.jpg", "hero.png");
            if (image != null) return image;

            image = TryGameAsset(game == null ? 0 : game.GameID,
                "cover.jpg", "cover.png", "poster.jpg", "poster.png");
            if (image != null) return image;

            return GeneratedFallback("banner", game, Math.Max(1600, fallbackWidth), Math.Max(650, fallbackHeight));
        }

        public static Image GetScreenshot(Game game, int index, int fallbackWidth, int fallbackHeight)
        {
            Image image = TryGameAsset(game == null ? 0 : game.GameID,
                "screenshot" + index + ".jpg",
                "screenshot" + index + ".png",
                "screen" + index + ".jpg",
                "screen" + index + ".png");
            return image ?? GetBanner(game, fallbackWidth, fallbackHeight);
        }

        public static Image GetLoginHero(int fallbackWidth, int fallbackHeight)
        {
            string folder = Path.Combine(AssetsRoot, "Login");
            string[] names = { "login-hero.jpg", "login-hero.png", "hero.jpg", "hero.png" };
            foreach (string name in names)
            {
                Image image = ImageCacheService.Load(Path.Combine(folder, name));
                if (image != null) return image;
            }
            return GeneratedFallback("login", null, Math.Max(1600, fallbackWidth), Math.Max(1000, fallbackHeight));
        }

        public static string GetGameFolder(int gameId)
        {
            return Path.Combine(AssetsRoot, "Games", gameId.ToString());
        }

        public static void ClearGenerated()
        {
            lock (Sync)
            {
                foreach (Image image in Generated.Values)
                    if (image != null) image.Dispose();
                Generated.Clear();
            }
        }

        private static Image TryGameAsset(int gameId, params string[] names)
        {
            if (gameId <= 0) return null;
            string folder = GetGameFolder(gameId);
            foreach (string name in names)
            {
                Image image = ImageCacheService.Load(Path.Combine(folder, name));
                if (image != null) return image;
            }
            return null;
        }

        private static Image GeneratedFallback(string kind, Game game, int width, int height)
        {
            int id = game == null ? 0 : game.GameID;
            string key = kind + ":" + id + ":" + width + "x" + height;
            lock (Sync)
            {
                Image image;
                if (Generated.TryGetValue(key, out image)) return image;

                if (string.Equals(kind, "login", StringComparison.OrdinalIgnoreCase))
                    image = ArtworkFactory.CreateLoginBackdrop(width, height);
                else if (string.Equals(kind, "banner", StringComparison.OrdinalIgnoreCase))
                    image = ArtworkFactory.CreateGameBanner(game, width, height);
                else
                    image = ArtworkFactory.CreateGameCover(game, width, height);

                Generated[key] = image;
                return image;
            }
        }
    }
}
