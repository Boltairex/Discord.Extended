using Discord.Commands;
using System;
using System.Collections.Generic;

namespace Discord.Extended
{
    /// <summary>
    /// Provides basic, often used methods and classes. 
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Used in Print methods.
        /// </summary>
        public static ConsoleColor BasicForeground = ConsoleColor.Gray;
        /// <summary>
        /// Used in Print methods.
        /// </summary>
        public static ConsoleColor BasicBackground = ConsoleColor.Black;

        /// <summary>
        /// Initialized once global <see cref="System.Random"/>.
        /// </summary>
        public static System.Random RandomSystem { get; private set; } = new System.Random();

        #region Numeric limits functions

        /// <summary>
        /// Limit in range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(this int value, int min, int max)
        {
            value.ClampRef(min, max);
            return value;
        }

        /// <summary>
        /// Limit in range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static void ClampRef(this ref int value, int min, int max)
        {
            value = value > max ? max : value < min ? min : value;
        }

        /// <summary>
        /// Limit in range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(this float value, float min, float max)
        {
            value.ClampRef(min, max);
            return value;
        }

        /// <summary>
        /// Limit in range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static void ClampRef(this ref float value, float min, float max)
        {
            value = value > max ? max : value < min ? min : value;
        }
       
        /// <summary>
        /// Limit in range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(this double value, double min, double max)
        {
            value.ClampRef(min, max);
            return value;
        }

        /// <summary>
        /// Limit in range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static void ClampRef(this ref double value, double min, double max)
        {
            value = value > max ? max : value < min ? min : value;
        }

        #endregion

        /// <summary>
        /// Creates Discord color from RGB values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Discord.Color CreateColorD(int r = 0, int g = 0, int b = 0) => (Discord.Color) CreateColor(r, g, b);

        /// <summary>
        /// Creates color from RGB values.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static System.Drawing.Color CreateColor(int r = 0, int g = 0, int b = 0, int a = 255)
        {
            return System.Drawing.Color.FromArgb(
                red: Clamp(r, 0, 255), 
                green: Clamp(g, 0, 255), 
                blue: Clamp(b, 0, 255),
                alpha: a);
        }

        /// <summary>
        /// Creates random Discord color.
        /// </summary>
        /// <returns></returns>
        public static Discord.Color GetRandomColorD() => (Discord.Color)GetRandomColor();

        /// <summary>
        /// Creates random color.
        /// </summary>
        /// <returns></returns>
        public static System.Drawing.Color GetRandomColor()
        {
            return System.Drawing.Color.FromArgb(
                red: RandomSystem.Next(1, 255),
                green: RandomSystem.Next(1, 255),
                blue: RandomSystem.Next(1, 255));
        }

        /// <summary>
        /// Checks user permission by using <see cref="SocketCommandContext"/>. Not secured.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool CheckUserPermission(this SocketCommandContext context, GuildPermission permission) => context.Guild.CurrentUser.GuildPermissions.Has(permission);

        /// <summary>
        /// Collects all user roles from specified guild.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IRole[] GetUserRoles(this IUser user, IGuild guild) {
            if (user == null)
                throw new System.ArgumentNullException($"{nameof(user)} is null");

            return GetUserRoles(guild.GetUserAsync(user.Id).Result);
        }

        /// <summary>
        /// Collects all roles from specified guild user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IRole[] GetUserRoles(this IGuildUser user)
        {
            if (user == null)
                throw new System.ArgumentNullException($"{nameof(user)} is null");

            List<IRole> roles = new List<IRole>();
            foreach (ulong u in user.RoleIds)
                roles.Add(user.Guild.GetRole(u));
            return roles.ToArray();
        }

        /// <summary>
        /// Prints <paramref name="text"/> with <see cref="ConsoleColor.Yellow"/> color and sets it after to <see cref="BasicForeground"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="prefix"/> - if set, it will be written before <paramref name="text"/>.
        /// </remarks>
        /// <param name="text"></param>
        /// <param name="prefix"></param>
        public static void PrintWarning(this string text, string prefix = null, string sufix = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (prefix != null && !string.IsNullOrWhiteSpace(prefix))
                Console.Write(prefix + " ");
            Console.WriteLine(text);
            if (sufix != null && !string.IsNullOrWhiteSpace(sufix))
                Console.WriteLine(sufix);
            Console.ForegroundColor = BasicForeground;
        }

        /// <summary>
        /// Just prints string like <see cref="Console.WriteLine"/>.
        /// </summary>
        /// <param name="text"></param>
        public static void Print(this string text) => Console.WriteLine(text);

        /// <summary>
        /// Prints <paramref name="text"/> with <paramref name="color"/> and sets it after to <see cref="BasicForeground"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void Print(this string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = BasicForeground;
        }

        /// <summary>
        /// Just prints with specified data.
        /// </summary>
        /// <remarks>If you use different colors in your project, set <see cref="BasicForeground"/> and <see cref="BasicBackground"/>.</remarks>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        public static void Print(this string text, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(text);
            Console.ForegroundColor = BasicForeground;
            Console.BackgroundColor = BasicBackground;
        }

        /// <summary>
        /// Safe to use for filenames on linux and windows.
        /// </summary>
        public static string GetCurrentDate { get => $"{DateTime.Now.ToString().Replace(':', '-').Replace('/', '-')}"; }

        /// <summary>
        /// Calculates strings similarity from 0.0 to 1.0.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static double CalculateSimilarity(this string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));

            /// <summary>
            /// Returns the number of steps required to transform the source string
            /// into the target string.
            /// </summary>
            int ComputeLevenshteinDistance(string source, string target)
            {
                if ((source == null) || (target == null)) return 0;
                if ((source.Length == 0) || (target.Length == 0)) return 0;
                if (source == target) return source.Length;

                int sourceWordCount = source.Length;
                int targetWordCount = target.Length;

                if (sourceWordCount == 0) return targetWordCount;
                if (targetWordCount == 0) return sourceWordCount;

                int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

                for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
                for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

                for (int i = 1; i <= sourceWordCount; i++)
                {
                    for (int j = 1; j <= targetWordCount; j++)
                    {
                        int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                        distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                    }
                }
                return distance[sourceWordCount, targetWordCount];
            }
        }


        /// <summary>
        /// Search for all used templates like {val1} to insert a value in any possible place (title, desc, fields, etc). Warning, can lead to overfill the embed length.
        /// </summary>
        /// <remarks>
        /// If value doesn't exist in <paramref name="values"/>, current template will be untouched.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static EmbedBuilder InsertValues(this EmbedBuilder builder, IReadOnlyDictionary<string, string> values)
        {
            builder.Title = TryFill(builder.Title, ref values);
            builder.Description = TryFill(builder.Description, ref values);
            builder.Url = TryFill(builder.Url, ref values);
            builder.ImageUrl = TryFill(builder.Url, ref values);
            builder.ThumbnailUrl = TryFill(builder.Url, ref values);
            builder.Footer.Text = TryFill(builder.Footer.Text, ref values);
            builder.Footer.IconUrl = TryFill(builder.Footer.IconUrl, ref values);
            for(int x = 0; x < builder.Fields.Count; x++)
            {
                builder.Fields[x].Value = TryFill(builder.Footer.IconUrl, ref values);
                builder.Fields[x].Name = TryFill(builder.Fields[x].Name, ref values);
            }

            string TryFill(string val, ref IReadOnlyDictionary<string, string> values)
            {
                if (string.IsNullOrEmpty(val)) return val;

                foreach (var replace in values)
                    val.Replace(replace.Key, replace.Value, StringComparison.CurrentCultureIgnoreCase);
                return val;
            }

            return builder;
        }

        /// <summary>
        /// Adds choices from the string array.
        /// </summary>
        /// <remarks>
        /// They will be created as <see cref="ApplicationCommandOptionType.Integer"/> in the same order as <paramref name="choices"/>.
        /// </remarks>
        /// <param name="optionName"></param>
        /// <param name="desc"></param>
        /// <param name="isRequired"></param>
        /// <param name="choices"></param>       
        /// <returns></returns>
        public static SlashCommandOptionBuilder FormChoices(string optionName, string desc, bool isRequired, params string[] choices)
        {
            SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder()
                .WithName(optionName)
                .WithDescription(desc)
                .WithRequired(isRequired)
                .WithType(ApplicationCommandOptionType.Integer);

            for (int x = 0; x < choices.Length && x < 25; x++)
                builder.AddChoice(choices[x], x);

            return builder;
        }
    }
}