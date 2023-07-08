using System;
using System.Collections.Generic;
using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.AI;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        public class BlackboardMenu
        {
                public static AIBase ai;

                public static void Open (AIBase newAI, List<string> dataList, int index)
                {
                        ai = newAI;
                        GenericMenu menu = new GenericMenu ( );
                        for (int i = 0; i < dataList.Count; i++)
                        {
                                if (index == 2 && !dataList[i].Contains ("Target/"))
                                {
                                        continue;
                                }
                                if (index == 3 && !dataList[i].Contains ("Territory/"))
                                {
                                        continue;
                                }
                                if (index == 4 && !dataList[i].Contains ("Variable/"))
                                {
                                        continue;
                                }
                                menu.AddItem (new GUIContent (dataList[i]), false, OnAddBlackboardTreeItem, dataList[i]);
                        }
                        menu.ShowAsContext ( );
                }

                private static void OnAddBlackboardTreeItem (object obj)
                {
                        if (ai == null) return;
                        string path = (string) obj;
                        string[] nameType = path.Split ('/');
                        CreateBlackboard (ai.data, nameType[0], nameType[nameType.Length - 1]);
                }

                public static Blackboard CreateBlackboard (List<Blackboard> children, string typeName, string nameType)
                {
                        if (ai == null) return null;

                        Blackboard node = null;
                        string actionType = "TwoBitMachines.FlareEngine.AI.BlackboardData." + nameType;
                        if (EditorTools.RetrieveType (actionType, out Type type))
                        {
                                node = ai.gameObject.gameObject.AddComponent (type) as Blackboard;
                                children.Add (node);
                                node.hideFlags = HideFlags.HideInInspector;
                                if (typeName == "Target")
                                {
                                        node.blackboardType = BlackboardType.Target;
                                }
                                else if (typeName == "Territory")
                                {
                                        node.blackboardType = BlackboardType.Territory;
                                }
                                else
                                {
                                        node.blackboardType = BlackboardType.Variable;
                                }
                        }
                        return node;
                }
        }
}