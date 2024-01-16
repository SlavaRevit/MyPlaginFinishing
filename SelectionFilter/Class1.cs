using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using MyRevitPlugin;
using VectorsAndPointsNew.Extension;
using static System.Windows.Forms.LinkLabel;
using static SelectionFilter.RailingsCreation;
using Curve = Autodesk.Revit.DB.Line;
using Line = Autodesk.Revit.DB.Line;
using Transform = Autodesk.Revit.DB.Transform;
namespace SelectionFilter
{

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class Class1 : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var app = uiapp.Application;
			var doc = uidoc.Document;

			//var selectedElements = uidoc.PickElements(x => x is Wall, PickElementsOptionFactory.CreateCurrentDocumentOption);
			var walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
			var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();
			using (Transaction t = new Transaction(doc, "Create Finishing in room"))
			{

				//var newForm =  new Form1();
				//newForm.ShowDialog();
				//var offset = double.Parse(newForm.Offset);
				//var height = double.Parse(newForm.Height);
				//var baseOffset = double.Parse(newForm.Base);
				t.Start();

				foreach (var r in rooms)
				{
					var boundarySegment = (SpatialElement)r;
					Room room = r as Room;
					// Get the geometry of the room
					var wallTypeElement = doc.GetElement(new ElementId(4910646));
					var widthWall = wallTypeElement.LookupParameter("Width").AsDouble();
					var roomLevelId = r.LevelId;
					var segments = boundarySegment.GetBoundarySegments(new SpatialElementBoundaryOptions());
					var DoorPoints = new List<XYZ>();
					var points = new List<XYZ>();
					var AllWidthOfDoors = new List<double>();
					var bLoc = (SpatialElementBoundaryLocation)Enum.Parse(typeof(SpatialElementBoundaryLocation), "Finish");
					var bOptions = new SpatialElementBoundaryOptions
					{
						SpatialElementBoundaryLocation = bLoc
					};
					var calculator = new SpatialElementGeometryCalculator(doc, bOptions);
					var NewResult = calculator.CalculateSpatialElementGeometry(room);
					var NewSegments = room.GetBoundarySegments(bOptions);
					var outerBoundaryCurves = new List<BoundarySegment>();
					var innerBoundaryCurves = new List<BoundarySegment>();
					var innerBoundaryCurvesAsCurve = new List<Autodesk.Revit.DB.Curve>();
					var curveLoopClosed = new CurveLoop();

					var profilenewchec = new CurveLoop();

					//getting inner and outer loops
					for (var i = 0; i < NewSegments.Count; i++)
					{
						if (i == 0)
						{
							outerBoundaryCurves = NewSegments[i].Select(x => x).ToList();
						}
						else
						{
							var elementIdsOfSegments = NewSegments[i]
									.Select(e => doc.GetElement(e.ElementId)).Where(d => d != null).ToList();

							var SegmentsWithDoor = elementIdsOfSegments
								.Select(e => doc.GetElement(e.Id)).Where(d => d != null)
								.Any(elem => elem.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance)))
								.Select(e => doc.GetElement(e))
								.Where(d => d.Category.Name == "Doors")
								.Any());

							if (SegmentsWithDoor)
							{
								innerBoundaryCurves.AddRange(NewSegments[i].Select(x => x));
							}
							else
							{
								innerBoundaryCurvesAsCurve.AddRange(NewSegments[i].Select(x => x.GetCurve()));
							}



						}
					}
					//TaskDialog.Show("iner by boundary", innerBoundaryCurves.Count.ToString());
					//TaskDialog.Show("iner by curve", innerBoundaryCurvesAsCurve.Count.ToString());


					//check for inner loops
					var ListOfLoopsToCreate = new List<List<BoundarySegment>>();
					var listOfInnerCurvesOneElement = new CurveLoop();
					HashSet<ElementId> visitedSegments = new HashSet<ElementId>();
					HashSet<Curve> visidetSegmentsAsCurve = new HashSet<Curve>();
					var listOfCurveLoops = new List<CurveLoop>();


					//WORKING PART FOR BOUNDARY SEGMENT
					void WorkingWithSegments()
					{
						for (var i = 0; i < innerBoundaryCurves.Count; i++)
						{
							//var curveLoop = new CurveLoop();
							if (innerBoundaryCurves[i] != null)
							{
								var segmentId = innerBoundaryCurves[i].ElementId;

								if (!visitedSegments.Contains(segmentId)) // If the segment is not visited
								{
									var innerLoop = new List<BoundarySegment>();
									TraverseConnectedSegments(innerBoundaryCurves[i], innerLoop, visitedSegments);

									ListOfLoopsToCreate.Add(innerLoop);
								}
							}
						}
					}

					void TraverseConnectedSegments(BoundarySegment segment, List<BoundarySegment> loop, HashSet<ElementId> visited)
					{
						var segmentId = segment.ElementId;
						if (visited.Contains(segmentId)) return;

						loop.Add(segment);
						visited.Add(segmentId);


						var curve = segment.GetCurve();
						var endPoint = curve.GetEndPoint(1);

						for (var j = 0; j < innerBoundaryCurves.Count; j++)
						{
							var otherSegment = innerBoundaryCurves[j];
							if (otherSegment != null)
							{
								var otherSegmentId = otherSegment.ElementId;
							}

							if (innerBoundaryCurves[j] != null) // Check if the segment is available
							{

								var otherCurve = innerBoundaryCurves[j].GetCurve();
								var otherStartPoint = otherCurve.GetEndPoint(0);
								var otherEndPoint = otherCurve.GetEndPoint(1);

								if (endPoint.IsAlmostEqualTo(otherStartPoint) || endPoint.IsAlmostEqualTo(otherEndPoint))
								{
									var nextSegment = innerBoundaryCurves[j];
									innerBoundaryCurves[j] = null; // Mark the segment as used
									TraverseConnectedSegments(nextSegment, loop, visited);
								}
							}
						}
					}

					//WORKING PART FOR CURVE LOOP
					void WorkingWithCurve()
					{
						for (var i = 0; i < innerBoundaryCurvesAsCurve.Count; i++)
						{
							var curve = innerBoundaryCurvesAsCurve[i];
							//var curveLoop = new CurveLoop();

							if (curve != null)
							{

								if (!visidetSegmentsAsCurve.Contains(innerBoundaryCurvesAsCurve[i])) // If the segment is not visited
								{
									var innerLoop = new CurveLoop();
									TraverseConnectedSegmentsAsCurve((Curve)innerBoundaryCurvesAsCurve[i], innerLoop, visidetSegmentsAsCurve);

									//curveLoop.Append(innerLoop); // Add the segments from innerLoop to curveLoop

									listOfCurveLoops.Add(innerLoop);
								}
							}
						}
					}
					//WORKIF PART FOR CURVES
					void TraverseConnectedSegmentsAsCurve(Curve segment, CurveLoop loop, HashSet<Curve> visited)
					{

						if (visited.Contains(segment)) return;
						loop.Append(segment);
						visited.Add(segment);
						var endPoint = segment.GetEndPoint(1);

						for (var j = 0; j < innerBoundaryCurvesAsCurve.Count; j++)
						{
							var otherSegment = innerBoundaryCurvesAsCurve[j];

							if (otherSegment != null)
							{

								var otherCurve = innerBoundaryCurvesAsCurve[j];
								var otherStartPoint = otherCurve.GetEndPoint(0);
								var otherEndPoint = otherCurve.GetEndPoint(1);

								if (endPoint.IsAlmostEqualTo(otherStartPoint) || endPoint.IsAlmostEqualTo(otherEndPoint))
								{
									var nextSegment = innerBoundaryCurvesAsCurve[j];
									//innerBoundaryCurves[j] = null; // Mark the segment as used
									TraverseConnectedSegmentsAsCurve((Curve)otherCurve, loop, visited);
								}

							}
						}
					}

					WorkingWithSegments();
					ListOfLoopsToCreate.Add(outerBoundaryCurves);

					#region can be deleted later
					//var show = listOfInnerLoops.Select(x => x).ToList();
					//var show2 = show.SelectMany(x => x.Select(x => x.ElementId)).ToList();
					//var window = new SimpleForm(show2);
					//window.Show();

					//var uniqueElementIds = innerBoundaryCurves.Select(c => c.ElementId).Distinct().ToList();

					//var elementIdsWithMultipleOccurrences = innerBoundaryCurves
					//	.GroupBy(c => c.ElementId)
					//	.Where(group => group.Count() > 1)
					//	.Select(group => group.Key)
					//	.ToList();


					//foreach (var currentElementId in elementIdsWithMultipleOccurrences)
					//{
					//	for (var i = 0; i < innerBoundaryCurves.Count; i++)
					//	{
					//		if (innerBoundaryCurves[i].ElementId == currentElementId)
					//		{
					//			profilenewchec.Append(innerBoundaryCurves[i].GetCurve());
					//		}
					//	}
					//	innerBoundaryCurves.RemoveAll(c => c.ElementId == currentElementId);
					//	var railing123 = Railing.Create(doc, profilenewchec, new ElementId(650981), roomLevelId);
					//	profilenewchec = new CurveLoop();
					//}


					//var resultNewInnerLoop = innerBoundaryCurves.Where(curve => !elementIdsWithMultipleOccurrences.Contains(curve.ElementId)).ToList();

					//var groupedByFirstFourNumbers = resultNewInnerLoop
					//	.GroupBy(
					//	curve => curve.ElementId.IntegerValue.ToString().Substring(0, 4),
					//	(key, curves) => curves.ToList())
					//	.ToList();
					#endregion

					WorkingWithCurve();


					foreach (var item in listOfCurveLoops)
					{

						var railing123 = Railing.Create(doc, item, new ElementId(650981), roomLevelId);
					}


					foreach (var innerSegments in ListOfLoopsToCreate)
					{

						var elementIdsOfSegments = innerSegments
							.Select(e => doc.GetElement(e.ElementId)).Where(d => d != null).ToList();


						var SegmentsWithDoor = elementIdsOfSegments
							.Select(e => doc.GetElement(e.Id)).Where(d => d != null)
							.Any(elem => elem.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance)))
							.Select(e => doc.GetElement(e))
							.Where(d => d.Category.Name == "Doors")
							.Any());


						HashSet<ElementId> processedSegmentIds = new HashSet<ElementId>();
						List<List<XYZ>> allSegmentPoints = new List<List<XYZ>>();
						if (SegmentsWithDoor)
						{
							var listofIds = new List<int>();

							foreach (var seg in innerSegments)
							{
								var elementIdOfSegment = seg.ElementId.IntegerValue;
								var seg_curve = seg.GetCurve();
								var direction = (seg_curve as Curve).Direction;
								List<XYZ> pointsOfSegment = new List<XYZ>();
								var listOfElementIdsWithDepElements = new List<ElementId>();
								if (elementIdOfSegment != -1)
								{
									//TaskDialog.Show("ID", seg.ElementId.ToString());
									var el_id = seg.ElementId;
									var element_seg = doc.GetElement(el_id);

									var depended_elements = element_seg.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance)))
										.Select(e => doc.GetElement(e))
										.Where(d => d.Category.Name == "Doors").ToList();
									var curveOfSegment = seg.GetCurve();
									var pointSegmentStart = curveOfSegment.GetEndPoint(0);
									pointsOfSegment.Add(pointSegmentStart);

									if (depended_elements.Any())
									{
										foreach (var element in depended_elements)
										{

											var door_type = element.GetTypeId();
											var door_elementType = doc.GetElement(door_type);
											var door_width = door_elementType.LookupParameter("Width").AsDouble();
											var doorLocationPoint = (doc.GetElement(element.Id).Location as LocationPoint).Point as XYZ;
											var curveLine = curveOfSegment as Curve;
											var directionOfSegmentLine = curveLine.Direction;

											var projectedPointToSegment = curveOfSegment.Project(doorLocationPoint).XYZPoint;

											Transform translationLeft = Transform.CreateTranslation(directionOfSegmentLine * (door_width / 2));
											Transform translationRight = Transform.CreateTranslation(-directionOfSegmentLine * (door_width / 2));
											var pointDoorLeft = translationLeft.OfPoint(projectedPointToSegment);
											var pointDoorRight = translationRight.OfPoint(projectedPointToSegment);
											AllWidthOfDoors.Add(door_width);
											pointsOfSegment.Add(pointDoorRight);
											pointsOfSegment.Add(pointDoorLeft);
										}
										processedSegmentIds.Add(el_id);
									}

								}
								else
								{
									var curveOfSegment = seg.GetCurve();
									var pointSegmentStart = curveOfSegment.GetEndPoint(0);
									pointsOfSegment.Add(pointSegmentStart);
								}

								pointsOfSegment.Sort((p1, p2) =>
								{
									// Calculate dot products of vector and points(to compare and sort them)
									double dotProduct1 = direction.DotProduct(p1);
									double dotProduct2 = direction.DotProduct(p2);
									return dotProduct1.CompareTo(dotProduct2);
								});

								allSegmentPoints.Add(pointsOfSegment);
							}



							// Combine all points of the segments into a single list
							List<XYZ> unifiedSortedPoints = allSegmentPoints.SelectMany(points => points).ToList();

							List<Curve> Lines = new List<Curve>();
							List<Curve> LinesCheck = new List<Curve>();

							for (int i = 0; i < unifiedSortedPoints.Count - 1; i++)
							{
								var line = Curve.CreateBound(unifiedSortedPoints[i], unifiedSortedPoints[i + 1]);
								//line.VisualizeCurve(doc);
								Lines.Add(line);
							}

							foreach (var line in Lines)
							{
								var lineLength = line.Length;
								if (AllWidthOfDoors.Any(doorWidth => Math.Abs(lineLength - doorWidth) < 0.05)) { continue; }
								else { LinesCheck.Add(line); };
							}

							//unifiedSortedPoints.Last().VisualizeAsPoint(doc);
							//unifiedSortedPoints.First().VisualizeAsPoint(doc);
							Curve lastline = Curve.CreateBound(unifiedSortedPoints.Last(), unifiedSortedPoints.First());
							LinesCheck.Add(lastline);


							var result = CheckLines(doc, LinesCheck);

							foreach (var re in result)
							{
								var railing = Railing.Create(doc, re, new ElementId(650981), roomLevelId);
							}
						}


					}


				}

				#region part of creating walls, ceiling, floors
				// revisit this part later
				//var createOffset = curveOfSegment.CreateOffset(widthWall/2, new XYZ(0, 0, -1));
				//var offsetOfCurveForRailing = curveOfSegment.CreateOffset(widthWall, new XYZ(0, 0, -1));
				//var wall = Wall.Create(doc, createOffset, new ElementId(4910646), roomLevelId,10,0,false, false);
				//profileSegment.Append(curveOfSegment);
				//var floor = Floor.Create(doc, new List<CurveLoop> { profileSegment }, new ElementId(213345), roomLevelId);
				//var railing = Railing.Create(doc, profileSegment_CHECK, new ElementId(650981), roomLevelId);
				//var ceiling = Ceiling.Create(doc, new List<CurveLoop> { profileSegment }, new ElementId(213585), roomLevelId) ;
				//var offsetParam = railing.LookupParameter("Offset from Path").Set(widthWall);
				//var ceilingOffset = ceiling.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM).Set(10);
				#endregion

				t.Commit();
			}


			return Result.Succeeded;
		}

		private List<CurveLoop> CheckLines(Document doc, List<Curve> curves)
		{
			var mainLoop = new List<CurveLoop>();
			HashSet<Curve> visitedCurve = new HashSet<Curve>();

			for (int i = 0; i < curves.Count; i++)
			{
				if (!visitedCurve.Contains(curves[i]))
				{

					var curveLoop = new CurveLoop();
					var firstLine = curves[0];
					var currentCurve = curves[i];
					var currentCurveStart = currentCurve.GetEndPoint(0);
					var currentCurveEnd = currentCurve.GetEndPoint(1);

					curveLoop.Append(currentCurve);
					visitedCurve.Add(currentCurve);

					bool foundConnected = true;

					while (foundConnected)
					{
						foundConnected = false;
						for (var j = 0; j < curves.Count; j++)
						{
							if (!visitedCurve.Contains(curves[j]))
							{
								var nextCurve = curves[j];
								var nextCurveStart = nextCurve.GetEndPoint(0);
								var nextCurveEnd = nextCurve.GetEndPoint(1);

								if (currentCurveEnd.IsAlmostEqualTo(nextCurveStart))
								{
									curveLoop.Append(nextCurve);
									visitedCurve.Add(nextCurve);
									currentCurve = nextCurve;
									currentCurveEnd = nextCurveEnd;
									foundConnected = true;
									break; // move to the next iteration after adding a connected curve
								}


								else if (currentCurveEnd.IsAlmostEqualTo(nextCurveEnd))
								{
									curveLoop.Append(Line.CreateBound(nextCurveEnd, nextCurveStart));
									visitedCurve.Add(nextCurve);
									currentCurve = nextCurve;
									currentCurveEnd = nextCurveStart;
									foundConnected = true;
									break;
								}

							}

						}

					}
					mainLoop.Add(curveLoop);
					var lastLoop = mainLoop.LastOrDefault();
					var firstLoop = mainLoop.FirstOrDefault();

					if (lastLoop != null && firstLoop != null)
					{
						var lastCurveEnd = lastLoop.Last().GetEndPoint(1);
						var firstCurveStart = firstLoop.First().GetEndPoint(0);

						if (lastCurveEnd.IsAlmostEqualTo(firstCurveStart))
						{
							var combinedLoop = new CurveLoop();
							// Remove last curve loop from mainLoop
							foreach (var curve in lastLoop)
							{
								combinedLoop.Append(curve);
							}
							foreach (var curve in firstLoop)
							{
								combinedLoop.Append(curve);
							}


							// Remove last and first curve loops from mainLoop
							mainLoop.Remove(lastLoop);
							mainLoop.Remove(firstLoop);
							mainLoop.Add(combinedLoop);

						}

					}

				}
			}
			return mainLoop;
		}

	}

}

