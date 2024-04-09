using Avalonia.Media;

namespace RedisViewDesktop.Helpers
{
    public class GradientHelper
    {
        public static readonly GradientColor[] Colors =
        [
            new GradientColor("#ff9a9e","#fad0c4"),
            new GradientColor("#a18cd1","#fbc2eb"),
            new GradientColor("#fad0c4","#ffd1ff"),
            new GradientColor("#ffecd2","#fcb69f"),
            new GradientColor("#ff9a9e","#fecfef"),
            new GradientColor("#f6d365","#fda085"),
            new GradientColor("#fbc2eb","#a6c1ee"),
            new GradientColor("#fdcbf1","#e6dee9"),
            new GradientColor("#cfd9df","#e2ebf0"),
            new GradientColor("#a6c0fe","#f68084"),
            new GradientColor("#fccb90","#d57eeb"),
            new GradientColor("#f093fb","#f5576c"),
            new GradientColor("#30cfd0","#330867"),
            new GradientColor("#d299c2","#fef9d7"),
            new GradientColor("#89f7fe","#66a6ff"),
            new GradientColor("#667eea","#764ba2"),
            new GradientColor("#6a11cb","#2575fc"),
            new GradientColor("#fff1eb","#ace0f9"),
            new GradientColor("#48c6ef","#6f86d6"),
            new GradientColor("#c471f5","#fa71cd"),
            new GradientColor("#feada6","#f5efef"),
            new GradientColor("#e9defa","#fbfcdb"),
            new GradientColor("#00c6fb","#005bea"),
            new GradientColor("#d9afd9","#97d9e1"),
            new GradientColor("#88d3ce","#6e45e2"),
            new GradientColor("#7028e4","#e5b2ca"),
            new GradientColor("#13547a","#80d0c7"),
            new GradientColor("#ff758c","#ff7eb3"),
            new GradientColor("#c71d6f","#d09693"),
            new GradientColor("#e8198b","#c7eafd"),
            new GradientColor("#f794a4","#fdd6bd"),
            new GradientColor("#209cff","#68e0cf"),
            new GradientColor("#e6b980","#eacda3"),
            new GradientColor("#bdc2e8","#e6dee9"),
            new GradientColor("#cc208e","#6713d2"),
            new GradientColor("#0acffe","#495aff"),
            new GradientColor("#b224ef","#7579ff"),
            new GradientColor("#ec77ab","#7873f5"),
            new GradientColor("#B6CEE8","#F578DC"),
            new GradientColor("#CBBACC","#2580B3")
        ];

        public static GradientColor GradientColor(int i)
        {
            int index = i % Colors.Length;
            return Colors[index];
        }

    }

    public class GradientColor(string colorStart, string colorStop)
    {
        public Color ColorStart { get; } = Color.Parse(colorStart);
        public Color ColorStop { get; } = Color.Parse(colorStop);
    }
}
