using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (PickUpItems))]
        public class PickUpItemsEditor : UnityEditor.Editor
        {
                private PickUpItems main;
                private SerializedObject parent;

                private void OnEnable ( )
                {
                        main = target as PickUpItems;
                        parent = serializedObject;
                        Layout.Initialize ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);

                        parent.Update ( );

                        AddInventories ( );

                        AddEvents ( );

                        Vendor ( );

                        FoldOut.BoxSingle (1, FoldOut.boxColor);
                        parent.FieldAndEnable ("Mouse Pick Up", "layer", "enableMouseClick");
                        Layout.VerticalSpacing (2);

                        parent.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (10);
                }

                public void AddInventories ( )
                {
                        if (!FoldOut.Bar (parent).Label ("Inventories", FoldOut.titleColor, FoldOut.titleBold).BR ("addInventory", execute : parent.Bool ("inventoryFoldOut")).FoldOut ("inventoryFoldOut")) return;

                        SerializedProperty array = parent.Get ("inventories");
                        if (parent.ReadBool ("addInventory"))
                        {
                                array.arraySize++;
                        }

                        for (int i = 0; i < array.arraySize; i++)
                        {
                                if (array.Element (i).FieldFullAndButtonBox ("Delete", FoldOut.boxColor, space : 2))
                                {
                                        array.DeleteArrayElement (i);
                                        break;
                                }
                        }
                }

                public void AddEvents ( )
                {
                        if (!FoldOut.Bar (parent, FoldOut.boxColor).Label ("Item Events", FoldOut.titleColor, FoldOut.titleBold).BR ("addEvent", execute : parent.Bool ("eventFoldOut")).FoldOut ("eventFoldOut")) return;

                        SerializedProperty array = parent.Get ("events");
                        if (parent.ReadBool ("addEvent"))
                        {
                                array.arraySize++;
                        }

                        Layout.VerticalSpacing (1);
                        for (int i = 0; i < array.arraySize; i++)
                        {
                                SerializedProperty element = array.Element (i);
                                FoldOut.BoxSingle (1, FoldOut.boxColor);
                                if (element.FieldAndButton ("Key Name", "keyName", "Delete"))
                                {
                                        array.DeleteArrayElement (i);
                                        break;
                                }
                                Fields.EventFoldOut (element.Get ("itemEvent"), element.Get ("foldOut"), "Item Event");
                                Layout.VerticalSpacing (1);
                        }

                }

                public void Vendor ( )
                {
                        if (FoldOut.Bar (parent, FoldOut.boxColor).Label ("Is Vendor", FoldOut.titleColor, FoldOut.titleBold).BRE ("isVendor").FoldOut ("vendorFoldOut"))
                        {
                                GUI.enabled = parent.Bool ("isVendor");
                                FoldOut.Box (2, FoldOut.boxColor, extraHeight : 5, yOffset: -2);
                                parent.Field ("Player Transform", "player");
                                parent.Field ("Player Money", "playerMoney");

                                bool open = FoldOut.FoldOutButton (parent.Get ("vendorEventsFoldOut"));

                                Fields.EventFoldOut (parent.Get ("onSell"), parent.Get ("onSellFoldOut"), "On Sell", execute : open);
                                Fields.EventFoldOut (parent.Get ("onReject"), parent.Get ("onRejectFoldOut"), "On Reject", execute : open);
                                GUI.enabled = true;
                        }
                }
        }
}