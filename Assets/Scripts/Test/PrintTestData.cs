using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class PrintTestData : MonoBehaviour {
	ScreenOrientation originSOrientation;
    // Use this for initialization
    
    string data = "<0;name:Teh Manis;price:10000;production_date:15/03/2017;id:2><1;name:Teh Manis;price:10000;production_date:16/03/2017;id:3<;2>name:Product New;price:121222;production_date:19/04/2017;id:4<;3>name:Chair;price:10000;production_date:19/04/2017;id:5<;4>name:Example 3;price:1000;production_date:05/04/2017;id:6<;5>name:Ayam Goreng;price:121;production_date:06/04/2017;id:7<<<";
    void Start () {
        //originSOrientation = Screen.orientation;
        //InvokeRepeating("PrintData", 0.5f, 3f);

        TreeNode<string> tree = parser(data, "root");

        string[] path = { "data", "2", "name" };
        TreeNode<string> valueTree = find(tree, path);

        if (valueTree == null)
        {
            Debug.Log("Data not found");
        }
        else
        {
            Debug.Log(getValueFromKey(valueTree.getValue()));
        }
    }
	
	// Update is called once per frame
	void Update () {
		//PrintDataOnce();
	}

	void PrintDataOnce()
	{
		if (originSOrientation != Screen.orientation) {
			Debug.Log ("# prev: " + Screen.orientation + " ; now: " + Screen.orientation);
			originSOrientation = Screen.orientation;
		}
	}

	void PrintData () {
		float[] result1 = new float[7];
		Kudan.AR.NativeInterface.ArbiTrackGetPose (result1);
		Quaternion ARBorientation = new Quaternion (result1 [3], result1 [4], result1 [5], result1 [6]); // The current orientation of the floor in 3D space, relative to the devic
		Vector3 rotation = ARBorientation.eulerAngles;
		Debug.Log("# current orientation: "+Screen.orientation + " ; rot1: "+rotation+" ; rot: "+transform.eulerAngles);
	}

    public TreeNode<string> find(TreeNode<string> tree, string[] words)
    {
        if (words.Length == 0)
        {
            return tree;
        }

        List<TreeNode<string>> childNodes = tree.getChildren();
        for (int j = 0; j < childNodes.Count; j++)
        {
            if (words.Length == 1)
            {
                string[] keyValuePair = childNodes[j].getValue().TrimStart(':').Split(':');
                if (keyValuePair[0].Equals(words[0]))
                {
                    return childNodes[j];
                }
            }
            else if (childNodes[j].getValue().Equals(words[0]))
            {

                return this.find(childNodes[j], words.Skip(1).ToArray());
            }
        }



        return null;
    }

    public TreeNode<string> parser(string data, string value)
    {
        TreeNode<string> treeNode = new TreeNode<string>(value);
        string prevString; int nextBack; int prevCress = -1;
        string nextString; TreeNode<string> childNode;
        for (int i = 0; i < data.Length; i++)
        {

            string a = data.ToCharArray(i, 1)[0].ToString();

            if (a.Equals(";"))
            {
                prevString = new string(data.ToCharArray(prevCress + 1, i - prevCress - 1));
                nextString = new string(data.ToCharArray(i + 1, data.Length - i - 1));
                childNode = new TreeNode<string>(prevString);
                treeNode.AddChild(childNode);
                prevCress = i;

            }
            else if (a.Equals(">"))
            {
                prevString = new string(data.ToCharArray(prevCress + 1, i - prevCress - 1));

                nextString = new string(data.ToCharArray(i + 1, data.Length - i - 1));
                nextBack = nextString.LastIndexOf("<");
                nextString = nextString.Substring(0, nextBack);

                treeNode.AddChild(parser(nextString, prevString));

                prevCress = i;
                //skip
                i = nextBack + 1;
                break;
            }
            else if (a.Equals("<"))
            {
                break;
            }
        }

        return treeNode;

    }

    public string getValueFromKey(string keyValuePair)
    {
        string[] keyValuePairs = keyValuePair.TrimStart(':').Split(':');
        if (keyValuePairs.Length == 2)
        {
            return keyValuePairs[1];
        }
        return null;
    }
}


public class TreeNode<T>
{
    private readonly T _value;
    private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

    public TreeNode(T value)
    {
        _value = value;
    }

    public TreeNode<T> this[int i]
    {
        get { return _children[i]; }
    }

    public TreeNode<T> Parent { get; private set; }

    public T Value { get { return _value; } }

    public T getValue()
    {
        return _value;
    }

    public TreeNode<T> AddChild(TreeNode<T> node)
    {
        _children.Add(node);
        return node;
    }

    public List<TreeNode<T>> getChildren()
    {
        return _children;
    }

    public int getTotalChildren()
    {
        return _children.Count;
    }


    public TreeNode<T> GetChildrenByName(string name)
    {
        for (int i = 0; i < _children.Count; i++)
        {
            if (_children[i].getValue().Equals(name))
            {
                return _children[i];
            }
        }

        return null;
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return _children.Remove(node);
    }

    public void Traverse(Action<T> action)
    {
        action(Value);
        foreach (var child in _children)
            child.Traverse(action);
    }

    public IEnumerable<T> Flatten()
    {
        return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
    }
}
