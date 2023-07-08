  #if UNITY_EDITOR
  using System.Collections.Generic;
  using UnityEditor;

  namespace TwoBitMachines.Editors
  {
          public static class Arrays
          {
                  public static void CreateNameList (this SerializedProperty array, List<string> names)
                  {
                          names.Clear ( );
                          for (int i = 0; i < array.arraySize; i++)
                          {
                                  names.Add (array.GetArrayElementAtIndex (i).FindPropertyRelative ("name").stringValue);
                          }
                          // names.Sort ( );
                  }

                  public static SerializedProperty CreateNewElement (this SerializedProperty array)
                  {
                          array.arraySize++;
                          SerializedProperty element = array.GetArrayElementAtIndex (array.arraySize - 1);
                          EditorTools.ClearProperty (element);
                          return element;
                  }

                  public static void DeleteArrayElement (this SerializedProperty array, int deleteIndex)
                  {
                          for (int i = 0; i < array.arraySize; i++)
                          {
                                  if (deleteIndex == i)
                                  {
                                          array.MoveArrayElement (i, array.arraySize - 1);
                                          array.arraySize--;
                                          return;
                                  }
                          }

                  }
                  public static void DeleteNullElements (SerializedProperty property)
                  {
                          for (int i = property.arraySize - 1; i >= 0; i--)
                          {
                                  if (property.GetArrayElementAtIndex (i) == null)
                                  {
                                          property.DeleteArrayElementAtIndex (i);
                                  }
                          }
                  }

                  public static SerializedProperty Element (this SerializedProperty array, int index)
                  {
                          return array.GetArrayElementAtIndex (index);
                  }

                  public static void InsertArrayElement (this SerializedProperty array, int index)
                  {
                          array.InsertArrayElementAtIndex (index);
                  }

                  public static SerializedProperty LastElement (this SerializedProperty array)
                  {
                          return array.GetArrayElementAtIndex (array.arraySize - 1);
                  }
          }
  }
  #endif