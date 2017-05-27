using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode {
	public TreeNode lowerChild,upperChild;
	public SortableObject piece;
	public TreeNode (SortableObject piece){
		this.piece = piece;
	}
}
