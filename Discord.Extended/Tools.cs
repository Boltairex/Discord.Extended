using Discord.Commands;
using System.Collections.Generic;

namespace Discord.Extended
{
    public static class Tools
    {
        public static System.Random RandomSystem = new System.Random();

        public static int Clamp(int value, int min, int max)
        {
            return value > max ? max : value < min ? min : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            return value > max ? max : value < min ? min : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            return value > max ? max : value < min ? min : value;
        }

        public static System.Drawing.Color CreateColor(int r = 0, int g = 0, int b = 0)
        {
            return System.Drawing.Color.FromArgb(
                red: Clamp(r, 0, 255), 
                green: Clamp(g, 0, 255), 
                blue: Clamp(b, 0, 255));
        }

        public static System.Drawing.Color GetRandomColor()
        {
            return System.Drawing.Color.FromArgb(
                red: RandomSystem.Next(1, 255),
                green: RandomSystem.Next(1, 255),
                blue: RandomSystem.Next(1, 255));
        }

        public static bool CheckUserPermission(SocketCommandContext context, GuildPermission permission) => context.Guild.CurrentUser.GuildPermissions.Has(permission);

        public static IRole[] GetUserRoles(IUser user, IGuild guild) {
            return GetUserRoles(guild.GetUserAsync(user.Id).Result);
        }

        public static IRole[] GetUserRoles(IGuildUser user)
        {
            if (user == null)
                throw new System.ArgumentNullException($"{nameof(user)} is null");

            List<IRole> roles = new List<IRole>();
            foreach (ulong u in user.RoleIds)
                roles.Add(user.Guild.GetRole(u));
            return roles.ToArray();
        }
    }
}