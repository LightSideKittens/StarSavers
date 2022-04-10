#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;



///////////////////////////////////
/* // EXAMPLE OF QUICK SUBSCRIPTION
///////////////////////////////////

#if UNITY_EDITOR
public class _test_class
{
	[InitializeOnLoadMethod] //special unity's editor API attribute
	static void AddButtonOoToolBar() //should be a static
	{
		EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onLeftLayoutGUI += ( rect )=> { //you can subscribe to the left or right area

			if ( GUI.Button( rect, "GO MY 1 LAYOUT" ) ) Debug.Log( "Hello Unity!" );

		};
	}
	//Don't forget enable 'Use Custom Buttons' toggles in the top bar settings
}
#endif

///////////////////////////////////
*/ ////////////////////////////////
   ///////////////////////////////////



namespace Examples.HierarchyPlugin
{

    //Don't forget enable 'Use Custom Buttons' toggles in the top bar settings
    public class CustomTopBarOnGUI
    {

        [InitializeOnLoadMethod]
		static void CreateEvents() //should be a static
		{
            //Don't forget enable 'Use Custom Buttons' toggles in the top bar settings
            EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onLeftLayoutGUI += OnLeftLayoutGUI;
			EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onRightLayoutGUI += OnRightLayoutGUI;
		}

        static void OnLeftLayoutGUI( Rect rect )
        {
            //Don't forget enable 'Use Custom Buttons' toggles in the top bar settings
            if ( GUI.Button( rect, "GO MY 1 LAYOUT" ) )
            {
                Debug.Log( "Hello Unity!" );
            }
        }

        static void OnRightLayoutGUI( Rect rect )
        {
            //Don't forget enable 'Use Custom Buttons' toggles in the top bar settings
            if ( GUI.Button( rect, "GO MY 2 LAYOUT" ) )
            {
                Debug.Log( "Hello Unity!" );
            }
        }
	}
}
#endif
