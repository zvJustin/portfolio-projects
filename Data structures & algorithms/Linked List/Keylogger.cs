using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KeyLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            List<string> input = new List<string>();

            for (int i = 0; i < n; i++)
            {
                input.Add(Console.ReadLine());
            }

            foreach (string s in input)
            {
                LinkedList linkedList = new LinkedList();
                linkedList.cursor = 0;
                linkedList.count = 0;
                linkedList.oldcursor = 0;
                Node currentNode = new Node(null);

                for (int k = 0; k < s.Length; k++)
                {
                    switch (s[k])
                    {
                        case '<':
                            if (linkedList.cursor > 0)
                            {
                                linkedList.cursor = linkedList.cursor - 1;
                            }
                            break;

                        case '>':
                            if (linkedList.cursor < linkedList.count)
                            {
                                linkedList.cursor = linkedList.cursor + 1;
                            }
                            break;

                        case '-':
                            if (linkedList.cursor == 0)
                            {
                                break;
                            }

                            currentNode = linkedList.DeleteNode(currentNode);
                            linkedList.count -= 1;
                            linkedList.cursor -= 1;
                            linkedList.oldcursor = linkedList.cursor;
                            break;

                        default:
                            if (linkedList.cursor == linkedList.count)
                            {
                                currentNode = linkedList.AppendNode(s[k].ToString(), currentNode);
                                linkedList.count += 1;
                                linkedList.cursor += 1;
                                linkedList.oldcursor = linkedList.cursor;
                                break;
                            }

                            currentNode = linkedList.AppendNodeAtC(s[k].ToString(), linkedList.cursor, currentNode);
                            linkedList.count += 1;
                            linkedList.cursor += 1;
                            linkedList.oldcursor = linkedList.cursor;
                            break;
                    }
                }

                linkedList.printList();
            }
        }
    }

    public class LinkedList
    {
        public uint count { get; set; }
        public uint cursor { get; set; }
        public uint oldcursor { get; set; }
        Node StartNode;

        public Node AppendNode(string v, Node n)
        {
            if (count == 0)
            {
                StartNode = new Node(v);
                return StartNode;
            }
            else
            {
                Node current = n;
                if (current == null)
                {
                    current = StartNode;
                }

                while (current.NextNode != null)
                {
                    current = current.NextNode;
                }

                current.NextNode = new Node(v);
                current.NextNode.PrevNode = current;
                return current.NextNode;
            }
        }

        public Node prependNode(string v)
        {
            Node newStartNode = new Node(v);
            Node temp = new Node(StartNode.Value);
            temp.NextNode = StartNode.NextNode;

            if (StartNode.NextNode != null)
            {
                StartNode.NextNode.PrevNode = temp;
            }

            temp.PrevNode = newStartNode;
            newStartNode.NextNode = temp;
            StartNode = newStartNode;
            return StartNode;
        }

        public Node DeleteNode(Node n)
        {
            if (StartNode == null)
            {
                return null;
            }

            Node current = n;

            if (oldcursor == cursor)
            {
                current = n;
            }
            else if (oldcursor < cursor)
            {
                uint d = oldcursor;
                if (current == null)
                {
                    current = StartNode;
                    d += 1;
                }

                while (current.NextNode != null && d < cursor)
                {
                    current = current.NextNode;
                    d += 1;
                }
            }
            else
            {
                uint d = oldcursor;
                while (current.PrevNode != null && d > cursor)
                {
                    current = current.PrevNode;
                    d -= 1;
                }
            }

            if (current.PrevNode == null && current.NextNode != null)
            {
                StartNode = current.NextNode;
                StartNode.PrevNode = null;
                return null;
            }
            else if (current.NextNode == null && current.PrevNode != null)
            {
                current.PrevNode.NextNode = null;
                return current.PrevNode;
            }
            else if (current.NextNode == null && current.PrevNode == null)
            {
                StartNode = null;
                return StartNode;
            }
            else
            {
                current.PrevNode.NextNode = current.NextNode;
                current.NextNode.PrevNode = current.PrevNode;
                return current.PrevNode;
            }
        }

        public Node AppendNodeAtC(string v, uint c, Node n)
        {
            if (c == 0)
            {
                return prependNode(v);
            }

            Node current = n;

            if (oldcursor < cursor)
            {
                uint i = oldcursor;
                if (current == null)
                {
                    current = StartNode;
                    i += 1;
                }

                while (current.NextNode != null && i < cursor)
                {
                    current = current.NextNode;
                    i += 1;
                }
            }
            else
            {
                uint i = oldcursor;
                while (current.PrevNode != null && i > cursor)
                {
                    current = current.PrevNode;
                    i -= 1;
                }
            }

            if (current.NextNode == null)
            {
                return AppendNode(v, n);
            }

            Node temp = new Node(current.NextNode.Value);
            temp.NextNode = current.NextNode.NextNode;

            if (current.NextNode.NextNode != null)
            {
                current.NextNode.NextNode.PrevNode = temp;
            }

            current.NextNode = new Node(v);
            current.NextNode.NextNode = temp;
            current.NextNode.PrevNode = current;
            current.NextNode.NextNode.PrevNode = current.NextNode;

            return current.NextNode;
        }

        public void printList()
        {
            StringBuilder str = new StringBuilder();
            Node current = StartNode;

            if (current != null)
            {
                str.Append(current.Value);

                while (current.NextNode != null)
                {
                    str.Append(current.NextNode.Value);
                    current = current.NextNode;
                }
            }

            Console.WriteLine(str);
        }
    }

    public class Node
    {
        public Node(string v)
        {
            Value = v;
            NextNode = null;
            PrevNode = null;
        }

        public string Value { get; set; }
        public Node NextNode { get; set; }
        public Node PrevNode { get; set; }
    }
}