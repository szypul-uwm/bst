using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BST
{
    internal class Visualization
    {
        // node settings
        private const int circleRadius = 20;
        private Pen pen = new Pen(Color.Black, 2);
        private SolidBrush nodeBrush = new SolidBrush(Color.White);
        private SolidBrush selectedNodeBrush = new SolidBrush(Color.LightSalmon);

        // text settings
        private SolidBrush textBrush = new SolidBrush(Color.Black);
        private const int fontSize = 14;
        private Font font = new Font("Calibri", fontSize);

        private int selectedNodeID = 0;
        private BST.Node? selectedNode = null;
        private Dictionary<int, Point> nodePositionsDict = new Dictionary<int, Point>();

        private BST.Tree tree;
        private PictureBox pictureBox;

        public Visualization(BST.Tree tree, PictureBox pictureBox)
        {
            this.tree = tree;
            this.pictureBox = pictureBox;

            this.pictureBox.Paint += new PaintEventHandler(pictureBox_Paint);
            this.pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseClick);
        }

        public BST.Node? GetSelectedNode()
        {
            return this.selectedNode;
        }

        private void drawNodes(BST.Node node, int l, int r, int depth, Graphics g)
        {
            int c = (l + r) / 2;
            int y = 50 + (50 * depth);

            Point circleCenter = new Point(c, y);
            nodePositionsDict.Add(node.GetUID(), circleCenter);

            if (node.left != null)
            {
                int toC = (l + c) / 2;
                int toY = y + 50;
                g.DrawLine(pen, c, y, toC, toY);
                drawNodes(node.left, l, c, depth + 1, g);
            }
            if (node.right != null)
            {
                int toC = (c + r) / 2;
                int toY = y + 50;
                g.DrawLine(pen, c, y, toC, toY);
                drawNodes(node.right, c, r, depth + 1, g);
            }

            Rectangle circleRect = new Rectangle();
            circleRect.X = c - circleRadius;
            circleRect.Width = circleRadius * 2;
            circleRect.Y = y - circleRadius;
            circleRect.Height = circleRadius * 2;

            if (selectedNodeID == node.GetUID())
                g.FillEllipse(selectedNodeBrush, circleRect);
            else
                g.FillEllipse(nodeBrush, circleRect);
            g.DrawEllipse(pen, circleRect);

            g.DrawString(node.value.ToString(), font, textBrush, c - circleRadius + 5, y - (fontSize / 2) - 2);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if(this.tree.root != null)
            {
                this.nodePositionsDict.Clear();
                drawNodes(tree.root, pictureBox.Left, pictureBox.Right, 0, g);
            }
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Point clickPos = e.Location;
            selectedNodeID = 0;
            foreach (var (id, pos) in nodePositionsDict)
            {
                float distance = MathF.Sqrt(MathF.Pow(clickPos.X - pos.X, 2.0f) + MathF.Pow(clickPos.Y - pos.Y, 2.0f));
                if (distance <= circleRadius)
                {
                    selectedNodeID = id;
                    break;
                }
            }
            pictureBox.Refresh();

            selectedNode = tree.FindNodeWithID(selectedNodeID);
        }
    }
}
