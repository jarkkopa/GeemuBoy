namespace GameBoy.GB
{
    public interface IDisplay
    {
        public void DrawLine(int y, uint[] line);
        public void Render();
    }
}
