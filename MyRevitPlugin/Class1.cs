using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace MyRevitPlugin
{
	//public class Class1 : IExternalApplication
	//{
	//	public Result OnShutdown(UIControlledApplication application)
	//	{
	//		return Result.Succeeded;
	//	}

	//	public Result OnStartup(UIControlledApplication application)
	//	{
	//		RibbonPanel ribbonPanel = application.CreateRibbonPanel("MyRibbonPanel");
	//		// create a button to triger
	//		string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
	//		PushButtonData buttonData = new PushButtonData("cmdMyTest", "My Test", thisAssemblyPath, "MyRevitPlugin.MyTest");

	//		PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;
	//		pushButton.ToolTip = "Hello , this is my first revit C# pluggin";


	//		//BitMap Icon
	//		Uri urlimage = new Uri(@"C:\Users\gfhap\Desktop\Room.png");
	//		BitmapImage bitmapImage = new BitmapImage(urlimage);
	//		pushButton.LargeImage = bitmapImage;

	//		return Result.Succeeded;
	//	}
	//}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]

	public class MyTest : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var app = uiapp.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var doc = uidoc.Document;

			var builtInCategoryId = new ElementId(BuiltInCategory.OST_Walls);
			var builtInCategory = Enum.GetValues(typeof(BuiltInCategory))
				.OfType<BuiltInCategory>()
				.Where(x => (int)x == builtInCategoryId.IntegerValue);
			var window = new SimpleForm(builtInCategory);
			window.ShowDialog();
			//var element = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).First();
			
			//using (var transaction = new Transaction(doc, "Set values"))
			//{
			//	transaction.Start();

			//	element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("");
			//	transaction.Commit();
			//}





			return Result.Succeeded;
		}
	}
}
