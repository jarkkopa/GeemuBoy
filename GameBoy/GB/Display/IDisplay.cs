namespace GameBoy.GB
{
    public interface IDisplay
    {
        public void DrawLine(int y, byte[] line);
        public void Render();
    }
}
