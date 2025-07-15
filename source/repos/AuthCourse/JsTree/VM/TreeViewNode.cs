namespace JsTree.VM
{
    public class TreeViewNode
    {
        public TreeViewNode(string Id, string Parent, string Text)
        {
            this.Id = Id;
            this.Parent = Parent;
            this.Text = Text;
        }
        public string Id { get; set; }
        public string Parent { get; set; }

        public string Text { get; set; }
    }
}
