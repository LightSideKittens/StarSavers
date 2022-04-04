using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class TB_Window : ScriptableObject
	{


	}


	[CustomEditor( typeof( TB_Window ) )]
	class SETGUI_TopBar : MainRoot
	{

		internal static string set_text =  USE_STR + "TopBar (Tool Bar)";
		internal static string set_key = "USE_TOPBAR_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			_GUI( (IRepaint)this );
		}
		public static void _GUI( IRepaint w )
		{
			Draw.RESET( w );

			Draw.BACK_BUTTON( w );
			Draw.TOG_TIT( set_text, set_key , WIKI: WIKI_2_TOPBAR);
			Draw.Sp( 10 );
			using ( ENABLE( w ).USE( set_key ) )
			{


				/*string RIGHT = "Right";
				string LEFT=  "Left";*/
				string RIGHT = "Left";
				string LEFT=  "Right";
				//if ( p.par_e.TOPBAR_SWAP_LEFT_RIGHT )
				//{
				//	var t = RIGHT;
				//	RIGHT = LEFT;
				//	LEFT = t;
				//}




				using ( GRO( w ).UP( 0 ) )
				{
					Draw.TOG_TIT( "Layouts and Hot Buttons:" );


					using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG( "Swap Layout and Hot Buttons Areas", "TOPBAR_SWAP_LEFT_RIGHT" );
					}
					Draw.Sp( 10 );




					//using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG_TIT( "Draw Layouts Tab", "DRAW_TOPBAR_LAYOUTS_BAR" ,EnableRed: false);
						//Draw.TOG( "Draw layouts tab", "DRAW_TOPBAR_LAYOUTS_BAR" );
						using ( ENABLE( w ).USE( "DRAW_TOPBAR_LAYOUTS_BAR" ) )
						{
							Draw.TOG( "Draw cross for selected layout", "TOPBAR_LAYOUTS_DRAWCROSS" );
							Draw.FIELD( "Addition [Y] top border adjustment", "TOPBAR_LAYOUTS_MIN_Y_OFFSET", -500, 500 );
							Draw.FIELD( "Addition [Y] bottom border adjustment", "TOPBAR_LAYOUTS_MAN_Y_OFFSET", -500, 500 );

							Draw.TOG( "Autosave selected layout (when you switch to another)", "TOPBAR_LAYOUTS_AUTOSAVE" );
							using ( ENABLE( w ).USE( "TOPBAR_LAYOUTS_AUTOSAVE" ) )
							{
								Draw.TOG( "Disable autosave for internal layout", "TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM" );
								Draw.HELP( w, "Be careful, because you can rewrite even a default layout, however you can of course reset it" );
							}

							Draw.HELP( w, "You can share your layouts that are stored in the: " +
								Folders.DataGetterByType( Folders.CacheFolderType.SettingsData, Folders.DATA_SETTINGS_PATH_USE_DEFAULT ).GET_PATH_TOSTRING + "/.SavedLayouts/" );

							if ( GUI.Button( Draw.R, "Open " + LayoutsMod.ASSET_FOLDER ) )
								Settings.SETGUI_MODS_ENABLER.REV( LayoutsMod.d );


						}
					}





#if !TRUE
					Draw.Sp( 10 );
					//	using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG_TIT( "Draw External Mods HotButtons on TopBar", "DRAW_TOPBAR_HOTBUTTONS",EnableRed: false );
						using ( ENABLE( w ).USE( "DRAW_TOPBAR_HOTBUTTONS" ) )
						{
							Draw.FIELD( "TopBar Buttons Size", "TOPBAR_HOTBUTTON_SIZE", 3, 60, "px" );
							p.par_e.DrawHotButtonsArray();
						}
					}
#endif
					Draw.Sp( 3 );

				}


				Draw.Sp( 10 );


				//using ( GRO( w ).UP( 0 ) )
				{






					//Draw.Sp( 10 );
					//  Draw.HRx2();
					using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG_TIT( "" + RIGHT + " Area:" );

						Draw.FIELD( "" + RIGHT + " area [X] left border offset", "TOPBAR_LEFT_MIN_BORDER_OFFSET", -500, 500 );
						Draw.FIELD( "" + RIGHT + " area [X] right border offset", "TOPBAR_LEFT_MAX_BORDER_OFFSET", -500, 500 );

						Draw.Sp( 5 );

						Draw.TOG( "Use custom buttons for " + RIGHT.ToLower() + " area", "DRAW_TOPBAR_CUSTOM_LEFT" );
						Draw.HELP( w, "You can add your gui at the top bars, using EMX." + Root.CUST_NS + "" );
						Draw.Sp( 5 );

					}


					Draw.Sp( 10 );
					//Draw.HRx2();
					//GUI.Label( Draw.R, "" + LEFT + " Area:" );
					using ( GRO( w ).UP( 0 ) )
					{
						Draw.TOG_TIT( "" + LEFT + " Area:" );

						Draw.FIELD( "" + LEFT + " area [X] left border offset", "TOPBAR_RIGHT_MIN_BORDER_OFFSET", -500, 500 );
						Draw.FIELD( "" + LEFT + " area [X] right border offset", "TOPBAR_RIGHT_MAX_BORDER_OFFSET", -500, 500 );


						Draw.Sp( 5 );


						Draw.TOG( "Use custom buttons for " + LEFT.ToLower() + " area ", "DRAW_TOPBAR_CUSTOM_RIGHT" );
						Draw.HELP( w, "You can add your gui at the top bars, using EMX." + Root.CUST_NS + "" );
						Draw.Sp( 4 );
					}
					//	Draw.Sp(10);

				}


				Draw.Sp( 10 );
				//Draw.HRx2();
				//GUI.Label( Draw.R, "" + LEFT + " Area:" );
				using ( GRO( w ).UP( 0 ) )
				{

					// Draw.TOG_TIT( "" + LEFT + " Area:" );

					Draw.TOG_TIT( "Quick tips:" );
					Draw.HELP_TEXTURE( w, "HELP_LAYOUT", 0 );
					Draw.HELP( w, "DRAG to change button position.", drawTog: true );
					Draw.HELP( w, "MMB to remove button, .", drawTog: true );
					Draw.HELP( w, "RMB to open menu.", drawTog: true );
					//Draw.HRx2();
					Draw.Sp( 10 );

					Draw.TOG_TIT( "You can add your own gui on topbar:", EnableRed: false );
					Draw.HELP( w, "Use ExtensionInterface_TopBarOnGUI.cs class", drawTog: true );
					Draw.Sp( 3 );
					if ( Draw.BUT( "Select Script with Custom Examples" ) ) { Selection.objects = new[] { Root.icons.example_folders[ 3 ] }; }
					Draw.Sp( 20 );

					//HOT BUTTONS

				}


			}
		}
	}
}
