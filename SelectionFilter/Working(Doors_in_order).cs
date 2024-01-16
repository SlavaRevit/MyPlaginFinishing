using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using SelectionFilter.BoundingBoxVisualizations;
using SelectionFilter.Collections;
using SelectionFilter.Enums;
using SelectionFilter.Extensions;
using SelectionFilter.Extensions.BoundingBoxes;
using SelectionFilter.Extensions.Solids;
using VectorsAndPointsNew.Extension;
using Curve = Autodesk.Revit.DB.Line;
using Line = Autodesk.Revit.DB.Line;
using Transform = Autodesk.Revit.DB.Transform;
namespace SelectionFilter
{

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class Working_Doors_in_order : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiapp = commandData.Application;
			var uidoc = uiapp.ActiveUIDocument;
			var app = uiapp.Application;
			var doc = uidoc.Document;
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
					var AllWidthOfDoors = new List<double>();
					var bLoc = (SpatialElementBoundaryLocation)Enum.Parse(typeof(SpatialElementBoundaryLocation), "Finish");
					var bOptions = new SpatialElementBoundaryOptions
					{
						SpatialElementBoundaryLocation = bLoc
					};
					var calculator = new SpatialElementGeometryCalculator(doc, bOptions);
					var NewSegments = room.GetBoundarySegments(bOptions);
					var outerBoundaryCurves = new List<BoundarySegment>();
					var innerBoundaryCurves = new List<BoundarySegment>();
					//var innerBoundaryCurvesAsCurve = new List<Autodesk.Revit.DB.Curve>();
					var innerBoundaryCurvesAsCurve = new List<Autodesk.Revit.DB.BoundarySegment>();
					var innerBoundaryCurvesForArc = new List<Autodesk.Revit.DB.BoundarySegment>();

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
								innerBoundaryCurvesAsCurve.AddRange(NewSegments[i].Select(x => x));
								//innerBoundaryCurvesForArc.AddRange(NewSegments[i].Select(x => x));
							}

						}
					}

					//check for inner loops
					var ListOfSegmentLoops = new List<List<BoundarySegment>>();
					HashSet<ElementId> visitedSegments = new HashSet<ElementId>();
					//HashSet<Curve> visidetSegmentsAsCurve = new HashSet<Curve>();
					HashSet<BoundarySegment> visidetSegmentsAsCurve = new HashSet<BoundarySegment>();
					HashSet<BoundarySegment> visidetSegmentsAsCurveArc = new HashSet<BoundarySegment>();
					var listOfCurveLoops = new List<CurveLoop>();
					var listOfCurveLoopsArc = new List<CurveLoop>();


					//WORKING PART FOR BOUNDARY SEGMENT (doesnt you it now, no need)
					void WorkingWithSegments()
					{
						for (var i = 0; i < innerBoundaryCurves.Count; i++)
						{

							if (innerBoundaryCurves[i] != null)
							{
								var segmentId = innerBoundaryCurves[i].ElementId;

								if (!visitedSegments.Contains(segmentId)) // If the segment is not visited
								{
									var innerLoop = new List<BoundarySegment>();
									TraverseConnectedSegments(innerBoundaryCurves[i], innerLoop, visitedSegments);

									ListOfSegmentLoops.Add(innerLoop);
								}
							}
						}
					}

					//doesnt use it now no need
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

					//WORKING PART FOR CURVE LOOP used for columns and single walls inside the room
					void WorkingWithCurve()
					{

						for (var i = 0; i < innerBoundaryCurvesAsCurve.Count; i++)
						{

							var curve = innerBoundaryCurvesAsCurve[i];

							//if (curve != null)
							//{

								if (!visidetSegmentsAsCurve.Contains(innerBoundaryCurvesAsCurve[i])) // If the segment is not visited
								{

									if (curve.GetCurve() is Arc arc)
									{
										var innerLoopArc = new CurveLoop();
									TraverseConnectedSegmentsAsCurveArc(innerBoundaryCurvesAsCurve[i], innerLoopArc, visidetSegmentsAsCurveArc);
										if (innerLoopArc.NumberOfCurves() > 0)
										{
											listOfCurveLoops.Add(innerLoopArc);
										}

									}
									else
									{
									var innerLoop = new CurveLoop();
										TraverseConnectedSegmentsAsCurve(curve, innerLoop, visidetSegmentsAsCurve);
										listOfCurveLoops.Add(innerLoop);
									}
								}
							//}
						}
					}

					//WORKIF PART FOR CURVES
					void TraverseConnectedSegmentsAsCurve(BoundarySegment segment, CurveLoop loop, HashSet<BoundarySegment> visited)
					{

						if (visited.Contains(segment)) return;
						loop.Append(segment.GetCurve());
						visited.Add(segment);
						var endPoint = segment.GetCurve().GetEndPoint(1);

						for (var j = 0; j < innerBoundaryCurvesAsCurve.Count; j++)
						{
							var otherSegment = innerBoundaryCurvesAsCurve[j];

							if (otherSegment != null)
							{

								//var otherSegment = innerBoundaryCurvesAsCurve[j];
								var otherCurve = innerBoundaryCurvesAsCurve[j].GetCurve();
								var otherStartPoint = otherCurve.GetEndPoint(0);
								var otherEndPoint = otherCurve.GetEndPoint(1);

								if (endPoint.IsAlmostEqualTo(otherStartPoint) || endPoint.IsAlmostEqualTo(otherEndPoint))
								{
									var nextSegment = innerBoundaryCurvesAsCurve[j];
									//innerBoundaryCurves[j] = null; // Mark the segment as used
									TraverseConnectedSegmentsAsCurve(otherSegment, loop, visited);
								}

							}
						}
					}

					void TraverseConnectedSegmentsAsCurveArc(BoundarySegment segment, CurveLoop loop, HashSet<BoundarySegment> visited)
					{

						if (visited.Contains(segment)) return;

						loop.Append(segment.GetCurve());
						visited.Add(segment);
						var endPoint = segment.GetCurve().GetEndPoint(1);

						for (var j = 0; j < innerBoundaryCurvesAsCurve.Count; j++)
						{
							var otherSegment = innerBoundaryCurvesAsCurve[j];

							//if (otherSegment != null)
							//{

								
								var otherCurve = innerBoundaryCurvesAsCurve[j].GetCurve();
								var otherStartPoint = otherCurve.GetEndPoint(0);
								var otherEndPoint = otherCurve.GetEndPoint(1);

								if (endPoint.IsAlmostEqualTo(otherStartPoint) || endPoint.IsAlmostEqualTo(otherEndPoint))
								{
									var nextSegment = innerBoundaryCurvesAsCurve[j];
									//innerBoundaryCurves[j] = null; // Mark the segment as used
									TraverseConnectedSegmentsAsCurveArc(otherSegment, loop, visited);
								}

							//}
						}
					}

					//WorkingWithSegments();
					WorkingWithCurve();


					ListOfSegmentLoops.Add(outerBoundaryCurves);
					ListOfSegmentLoops.Add(innerBoundaryCurves);


					foreach (var item in listOfCurveLoops)
					{

						var railing123 = Railing.Create(doc, item, new ElementId(650981), roomLevelId);
					}

					
					foreach (var SegmentLoop in ListOfSegmentLoops)
					{
						
						List<List<XYZ>> allSegmentPoints = new List<List<XYZ>>();
						List<XYZ> doorPoints = new List<XYZ>();
						HashSet<ElementId> processedSegmentIds = new HashSet<ElementId>();
						var listOfDoorPointOnArc = new List<XYZ>();
						var listStartPoints = new List<XYZ>();

						for (int i = 0; i < SegmentLoop.Count; i++)
						{
							var seg = SegmentLoop[i];
							List<XYZ> pointsOfSegment = new List<XYZ>();
							var elementIdOfSegment = seg.ElementId;
							var elementIdsOfSegments = SegmentLoop
								.Select(e => doc.GetElement(elementIdOfSegment)).Where(d => d != null).ToList();

							var SegmentsWithDoor = elementIdsOfSegments
								.Select(e => doc.GetElement(e.Id)).Where(d => d != null)
								.Any(elem => elem.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance)))
								.Select(e => doc.GetElement(e))
								.Where(d => d.Category.Name == "Doors")
								.Any());

							var element = doc.GetElement(elementIdOfSegment);

							var seg_result = seg.GetCurve();
							XYZ direction;
							if (seg_result is Arc arc)
							{
								var curve = (element.Location as LocationCurve).Curve;
								var directionX = (curve as Arc).XDirection;
								var directionY = (curve as Arc).YDirection;
								direction = new XYZ((directionX.X + directionY.X), (directionX.Y + directionY.Y), (directionX.Z + directionY.Z));
							}
							else
							{
								direction = (seg.GetCurve() as Curve).Direction;
							}


							if (SegmentsWithDoor)
							{
								
								var curveOfSegment = seg.GetCurve();
								var element_seg = doc.GetElement(elementIdOfSegment);
								var depended_elements = element_seg.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance)))
									.Select(e => doc.GetElement(e))
									.Where(d => d.Category.Name == "Doors").ToList();

								if (curveOfSegment is Arc)
								{
									//var pointSegmentStart = curveOfSegment.GetEndPoint(0);
									var centerPoint = (curveOfSegment as Arc).Tessellate();
									centerPoint.RemoveAt(centerPoint.Count - 1);
									foreach (var p in centerPoint)
									{
										pointsOfSegment.Add(p);
									}
								}
								else
								{
									var pointSegmentStart = curveOfSegment.GetEndPoint(0);
									pointsOfSegment.Add(pointSegmentStart);

								}

								foreach (var door in depended_elements)
								{
									var doorTransform = (door as Instance).GetTransform();
									var boundingBoxDoor = door.get_BoundingBox(doc.ActiveView);
									var curves = boundingBoxDoor.GetCurveLoop(FaceSide.Bottom);
									var result222 = curves.Select(x => x.Intersect(curveOfSegment)).ToList();
									foreach (var compr in result222)
									{
										if (compr == SetComparisonResult.Overlap)
										{
											var door_type = door.GetTypeId();
											var door_elementType = doc.GetElement(door_type);
											var door_width = door_elementType.LookupParameter("Width").AsDouble();
											var doorLocationPoint = (doc.GetElement(door.Id).Location as LocationPoint).Point as XYZ;
											var projectedPointToSegment = curveOfSegment.Project(doorLocationPoint).XYZPoint;
											if (curveOfSegment is Curve)
											{
												Transform translationLeft = Transform.CreateTranslation(direction.Normalize() * (door_width / 2));
												Transform translationRight = Transform.CreateTranslation(-direction.Normalize() * (door_width / 2));
												var pointDoorLeft = translationLeft.OfPoint(projectedPointToSegment);
												var pointDoorRight = translationRight.OfPoint(projectedPointToSegment);
												AllWidthOfDoors.Add(door_width);
												pointsOfSegment.Add(pointDoorLeft);
												pointsOfSegment.Add(pointDoorRight);
											
												break;
											}

											else if (curveOfSegment is Arc)
											{
												AllWidthOfDoors.Add(door_width);
												var arcCenter = (curveOfSegment as Arc).Center;
												var radius = (curveOfSegment as Arc).Radius;

												// Find the angle of the projected point relative to the arc's center
												double angleOfProjectedPoint = Math.Atan2(projectedPointToSegment.Y - arcCenter.Y,
																						  projectedPointToSegment.X - arcCenter.X);
												// Calculate the angles for the door points
												double halfDoorWidth = door_width / 2;
												double angleDelta = Math.Asin(halfDoorWidth / radius); // Angle offset for the door points
												double angleLeft = angleOfProjectedPoint - angleDelta;
												double angleRight = angleOfProjectedPoint + angleDelta;
												// Calculate the new positions
												var pointDoorLeft = new XYZ(arcCenter.X + radius * Math.Cos(angleLeft),
																			  arcCenter.Y + radius * Math.Sin(angleLeft),
																			  projectedPointToSegment.Z); // Assuming a flat arc (2D projection)

												var pointDoorRight = new XYZ(arcCenter.X + radius * Math.Cos(angleRight),
																			   arcCenter.Y + radius * Math.Sin(angleRight),
																			   projectedPointToSegment.Z); // Assuming a flat arc (2D projection)
												
												pointsOfSegment.Add(pointDoorLeft);
												pointsOfSegment.Add(pointDoorRight);

											
												listOfDoorPointOnArc.Add(pointDoorLeft);
												listOfDoorPointOnArc.Add(pointDoorRight);
												break;
												
											}
											
										}
										
									}
								}

							}

							else
							{
								var curveOfSegment = seg.GetCurve();
								if (curveOfSegment is Arc)
								{
									var centerPoint = (curveOfSegment as Arc).Tessellate();
									centerPoint.RemoveAt(centerPoint.Count - 1);
									foreach (var p in centerPoint)
									{
										pointsOfSegment.Add(p);
									}
								}
								else
								{
									var pointSegmentStart = curveOfSegment.GetEndPoint(0);
									pointsOfSegment.Add(pointSegmentStart);
								}

							}

							
							if (seg.GetCurve() is Arc arccheck)
							{
								var arcCenter = arccheck.Center;
								var arcNormal = arccheck.Normal;
								XYZ startPoint = arccheck.GetEndPoint(0);
								// Calculate the start angle
								double startAngle = Math.Atan2(startPoint.Y - arcCenter.Y, startPoint.X - arcCenter.X);
								// Normalize the start angle to be between 0 and 2*PI
								if (startAngle < 0)
									startAngle += 2 * Math.PI;
								ArcPointSorter sorter = new ArcPointSorter(arccheck,arcCenter,arcNormal,startAngle, true);
								var sortedPoints = sorter.SortPoints(pointsOfSegment).ToList();
								var PointsWithoutSmallDistance = checkPoints(doc, sortedPoints, listOfDoorPointOnArc);
								var resultedPoints = removeClosestPoints(PointsWithoutSmallDistance, listOfDoorPointOnArc, listStartPoints);

								allSegmentPoints.Add(resultedPoints);
							}
							else
							{
								
								pointsOfSegment.Sort((p1, p2) =>
									{
										// Calculate dot products of vector and points
										// it relevant only for segments in 2d ( not arcs )
										double dotProduct1 = direction.DotProduct(p1);
										double dotProduct2 = direction.DotProduct(p2);
										return dotProduct1.CompareTo(dotProduct2);
									});
								allSegmentPoints.Add(pointsOfSegment);
							}
							
						}

						
						////Combine all points of the segments into a single list
						List<XYZ> unifiedSortedPoints = allSegmentPoints.SelectMany(points => points).ToList();

						List<Curve> Lines = new List<Curve>();
						List<Curve> LinesCheck = new List<Curve>();
						for (int i = 0; i < unifiedSortedPoints.Count - 1; i++)
						{

							var line = Curve.CreateBound(unifiedSortedPoints[i], unifiedSortedPoints[i + 1]);

							Lines.Add(line);

						}

						foreach (var line in Lines)
						{
							var lineLength = line.Length;
							if (AllWidthOfDoors.Any(doorWidth => Math.Abs(lineLength - doorWidth) < 0.01)) { continue; }

							else
							{
								LinesCheck.Add(line);
							};
						}
						
						try
						{
							Curve lastline = Curve.CreateBound(unifiedSortedPoints.Last(), unifiedSortedPoints.First());
							LinesCheck.Add(lastline);
						}
						catch (Exception)
						{
							continue;
						}
						

						var result = CheckLines(doc, LinesCheck);

						foreach (var re in result)
						{
							var railing = Railing.Create(doc, re, new ElementId(650981), roomLevelId);
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

		private List<XYZ> checkPoints(Document doc, List<XYZ> points, List<XYZ> doorPoints)
		{
			if (points.Count < 2)
			{
				return points;
			}

			bool isInsideDoor = false;
			int i = 0;

			while (i < points.Count)
			{
				if (doorPoints.Contains(points[i]))
				{
					if (isInsideDoor)
					{
						// Keep the end point of the door, toggle the flag, and move to the next point
						isInsideDoor = false;
						i++;
					}
					else
					{
						// Keep the start point of the door, toggle the flag, and remove subsequent points
						isInsideDoor = true;
						i++;
						// Remove points inside the door
						while (i < points.Count && !doorPoints.Contains(points[i]))
						{
							points.RemoveAt(i);
						}
					}
				}
				else if (!isInsideDoor)
				{
					// Move to the next point if it's not inside a door
					i++;
				}
				// If the point is inside a door and not a door point, it will be removed
			}

			return points;
		}


		private List<XYZ> removeClosestPoints(List<XYZ> points, List<XYZ> doorPoints, List<XYZ> listStartPoints)
		{
			double tolerance = 0.001;
			int i = 0;

			while (i < points.Count - 1)
			{
				// Check if the current point is almost equal to the next one
				if (points[i].IsAlmostEqualTo(points[i + 1], tolerance))
				{
					// Check if neither the current point nor the next point is a door point
					if (!doorPoints.Contains(points[i]) && !doorPoints.Contains(points[i + 1]) && !listStartPoints.Contains(points[i]))
					{
						points.RemoveAt(i + 1); // Remove the next point
												// Do not increment 'i' as the next element has shifted to the current index
					}
					else
					{
						i++; // Increment 'i' if any point is a door point
					}
				}
				else
				{
					i++; // Increment 'i' if points are not almost equal
				}
			}

			return points;
		}


	}

}

////CHECK ALLL