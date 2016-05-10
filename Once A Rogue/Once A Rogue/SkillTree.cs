using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class SkillTree
    {
        private Button rootButton;
          
        public Button RootButton
        {
            get { return rootButton; }
            set { rootButton = value; }
        }

        public SkillTree(Button butt)
        {
            rootButton = butt;
        }

        public void Insert(string s, Button root)
        {
            ////Checking the data to make sure it is not the same as the root's data
            //if (s != root)
            //{
            //    //If the left child is null
            //    if (root.Child == null)
            //    {
            //        //Make the left child a node with the tossed in data
            //        root.LeftChild = new Node(data, null, null);
            //        return;
            //    }

            //    //If the right child is null and the left isn't
            //    else if (root.LeftChild != null && root.RightChild == null)
            //    {
            //        //
            //        root.RightChild = new Node(data, null, null);
            //        return;
            //    }

            //    //If both are full, insert run the method again on the right child
            //    else
            //    {
            //        Insert(data, root.RightChild);
            //    }
            //}
        }
    }
}
