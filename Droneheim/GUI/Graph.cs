using Droneheim.GUI.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	class KeyframeGraphLineEnumerator : IEnumerable<Vector2>, IEnumerator
	{
		protected Keyframes keyframes;
		protected int keyframe = 0;
		protected int from = 0;
		protected int to = 0;
		protected int resolution;

		public object Current => new Vector2(keyframe, keyframes.GetMagnitudeAt(keyframe));

		public KeyframeGraphLineEnumerator(Keyframes keyframes, int from, int to, int resolution = 1)
		{
			this.keyframes = keyframes;
			this.from = from;
			this.to = to;
			this.resolution = resolution;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			keyframe += resolution;
			return keyframe < to;
		}

		public void Reset()
		{
			keyframe = 0;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		public IEnumerator<Vector2> GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	class KeyframeGraphLine : GraphData
	{
		protected Keyframes keyframes;

		public event GraphLineChangeHandler OnChange;

		public float MinValue => keyframes.MinimumMagnitude;

		public float MaxValue => keyframes.MaximumMagnitude;

		public int NumPoints => keyframes.Count;

		public bool Visible => true;

		public KeyframeGraphLine(Keyframes keyframes)
		{
			this.keyframes = keyframes;
		}

		public IEnumerable<Vector2> GetPoints(int from, int to, int resolution = 1)
		{
			return new KeyframeGraphLineEnumerator(this.keyframes, from, to, resolution);
		}
	}

	public delegate void GraphLineChangeHandler(GraphData line);
	public interface GraphData
	{
		float MinValue { get; }
		float MaxValue { get; }
		int NumPoints { get; }
		IEnumerable<Vector2> GetPoints(int from, int to, int resolution = 1);
		bool Visible { get; }

		event GraphLineChangeHandler OnChange;
	}

	class GraphDataTest : GraphData
	{
		public float MinValue => 0;

		public float MaxValue => 100;

		public int NumPoints => 20;

		public bool Visible => true;

		public event GraphLineChangeHandler OnChange;

		public IEnumerable<Vector2> GetPoints(int from, int to, int resolution = 1)
		{
			List<Vector2> points = new List<Vector2>();
			for (int i = 0; i < NumPoints; i++)
			{
				//points.Add(new Vector2(i * 10, Mathf.Sin(i / 10.0f) * 50.0f + Mathf.Sin(i / 3.0f) * 20.0f));
				points.Add(new Vector2(i * 100, UnityEngine.Random.Range(-100, 100)));
			}
			return points;
		}
	}

	class GraphicGraphView : Graphic
	{
		protected LineRenderer2D lineRenderer = new LineRenderer2D();
		protected PointRenderer2D pointRenderer = new PointRenderer2D();
		protected Graph graph;
		protected Vector2Int bounds = new Vector2Int();

		public Graph Graph { 
			get { return graph; } 
			internal set
			{
				graph = value;
				SetAllDirty();
			} 
		}

		public GraphicGraphView()
		{
		}

		protected void RenderLine(GraphData line, LineRendererHelper helper)
		{
			lineRenderer.Render(helper, line.GetPoints(bounds.x, bounds.y).Cast<Vector2>().ToArray(), new LineRendererOptions() { Weight = 5 });
		}

		protected void RenderPoints(GraphData data, PointRendererHelper helper)
		{
			pointRenderer.Render(helper, data.GetPoints(bounds.x, bounds.y), new PointRendererOptions() { Weight = 5 });
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Render(vh);
		}

		public void Render(VertexHelper vh)
		{
			if (graph == null)
				return;

			vh.Clear();
			{
				LineRendererHelper helper = new LineRendererHelper(vh);
				foreach (GraphData data in graph.Data)
				{
					RenderLine(data, helper);
				}
			}

			{
				PointRendererHelper helper = new PointRendererHelper(vh);
				foreach (GraphData data in graph.Data)
				{
					//RenderPoints(data, helper);
				}
			}
		}
	}

	public enum GraphType
	{
		Line,
		Scatter,
		Bar
	}

	public enum GraphAxisType
	{
		Number,
		Time,
	}

	public class GraphAxis
	{
		public GraphAxisType Type;
		public string Legend;
		public int BoundMin;
		public int BoundMax;
	}

	public class Graph
	{
		protected List<GraphData> data = new List<GraphData>();
		public List<GraphData> Data => data;

		public GraphData AddData(GraphData data)
		{
			this.data.Add(data);
			data.OnChange += d => { };
			return data;
		}
	}

	public class LineGraph : Graph
	{
		protected List<GraphAxis> axis = new List<GraphAxis>();
		public List<GraphAxis> Axis => axis;
	}
}
