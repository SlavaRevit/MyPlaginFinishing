﻿using SelectionFilter.BoundingBoxVisualizations;
using System.Collections.Generic;


namespace SelectionFilter.Builders
{
	public class BoundingBoxVisualizationBuilder
	{
		private List<IBoundingBoxVisualization> _visualizations = new List<IBoundingBoxVisualization>();

		public BoundingBoxVisualizationBuilder AddSolidVisualization()
		{
			_visualizations.Add(new SolidBoundingBoxVisualization());
			return this;
		}
		public BoundingBoxVisualizationBuilder AddCornersVisualization()
		{
			_visualizations.Add(new CornersBoundingBoxVisualization());
			return this;
		}
		public BoundingBoxVisualizationBuilder AddCenterVisualization()
		{
			_visualizations.Add(new CenterBoundingBoxVisualization());
			return this;
		}
		public BoundingBoxVisualizationBuilder AddFacesVisualization()
		{
			_visualizations.Add(new FacesBoundingBoxVisualization());
			return this;
		}
		public BoundingBoxVisualizationBuilder AddTransformVisualization()
		{
			_visualizations.Add(new TransformBoundingBoxVisualization());
			return this;
		}
		public BoundingBoxVisualizationBuilder Add(IBoundingBoxVisualization visualization)
		{
			_visualizations.Add(visualization);
			return this;
		}

		public IBoundingBoxVisualization Build()
		{
			return new AggregateBoundingBoxVisualization(_visualizations.ToArray());
		}
	}
}

