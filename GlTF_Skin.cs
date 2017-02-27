﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlTF_Skin : GlTF_Writer {
	public List<string> boneNames;
	GlTF_Matrix bindShape;
	GlTF_Accessor ibmAccessor;

	public static string GetNameFromObject(Object o) 
	{		 		
		return "skin_" + GlTF_Writer.GetNameFromObject(o, true);
	}

	public void Populate(SkinnedMeshRenderer smr) {			
		name = GetNameFromObject(smr.transform);
		if (smr.rootBone != null) {
			boneNames = new List<string>();
			List<Matrix4x4> boneMats = new List<Matrix4x4>();

			var parent = smr.rootBone.parent;
			if (parent != null) {				
				var mat = Matrix4x4.TRS(parent.localPosition, parent.localRotation, parent.localScale);
				bindShape = new GlTF_Matrix(mat);
			} else {
				bindShape = new GlTF_Matrix(Matrix4x4.identity);
			}
			bindShape.name = "bindShapeMatrix";

			for (var i = 0; i < smr.bones.Length; ++i) {
				boneNames.Add(GlTF_Node.GetNameFromObject(smr.bones[i]));
				boneMats.Add(smr.sharedMesh.bindposes[i]);
			}

			ibmAccessor =  new GlTF_Accessor("accessor_ibm_" + name, GlTF_Accessor.Type.MAT4, GlTF_Accessor.ComponentType.FLOAT);
			ibmAccessor.bufferView = GlTF_Writer.mat4BufferView;
			ibmAccessor.Populate(boneMats.ToArray());
			GlTF_Writer.accessors.Add(ibmAccessor);
		}
	}		

	public override void Write()
	{
		Indent();		jsonWriter.Write ("\"" + name + "\": {\n");
		IndentIn();
		if (bindShape != null) 
		{
			CommaNL();
			bindShape.Write();
		}

		if (ibmAccessor != null)
		{
			CommaNL();
			Indent();	jsonWriter.Write("\"inverseBindMatrices\": \"" + ibmAccessor.name + "\"");
		}

		CommaNL();
		Indent();		jsonWriter.Write ("\"jointNames\": [\n");
		IndentIn();
		foreach (var s in boneNames)
		{
			CommaNL();
			Indent();	jsonWriter.Write("\"" + s + "\"");
		}
		IndentOut();
		jsonWriter.WriteLine();
		Indent();		jsonWriter.Write ("]\n");

		IndentOut();
		Indent();		jsonWriter.Write ("}");
	}
}
