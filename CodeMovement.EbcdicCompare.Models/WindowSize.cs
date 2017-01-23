namespace CodeMovement.EbcdicCompare.Models
{
    public class WindowSize
    {
        public WindowSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
