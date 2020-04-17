namespace GeemuBoy.GB
{
    public interface IDisplay
    {
        public void RenderLine(int y, uint[] line);
        public void Render();
    }
}
