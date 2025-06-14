// TODO: We wszystkich funkcjach z Renderer zrobić zabezpieczenie przed non-fullscreen mode

namespace Solitaire
{
    internal class Program
    {
        static void Main()
        {
            GameManager gameManager;

            while (true)
            {
                gameManager = new();
                gameManager.ShowStartMenu();
                gameManager.InitializeCardStacks();
                gameManager.Start();
            }
        }
    }
}