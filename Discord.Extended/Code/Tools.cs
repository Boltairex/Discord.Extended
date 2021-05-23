namespace Discord.Extended
{
    public static class Tools
    {
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
    }
}