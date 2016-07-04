using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Concurrent;
using System.Xml.Linq;
using HaiFeng;
using ToolTipEventArgs = System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs;
using Numeric = System.Decimal;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public partial class FormWorkSpace : Form
	{
		/// <summary>
		/// 工作区窗口
		/// </summary>
		/// <param name="pWorkSetting"></param>
		public FormWorkSpace(Strategy pStra)
		{
			InitializeComponent();
			_stra = pStra;
		}


		readonly List<OrderItem> _listOperateArrow = new List<OrderItem>();
		private readonly Strategy _stra;

		#region 变量
		//最大显示K线数
		private int maxShowCandleNum = 5000;

		//chart中显示的图形注释编号：自增
		ulong annotationID = 0;

		//行情变化队列：策略运行前
		private List<Tuple<bool, DateTime, double[], string[], double[], Strategy>> listQuoteChange = new List<Tuple<bool, DateTime, double[], string[], double[], Strategy>>();

		//序列变化队列：策略运行后
		private List<Tuple<DateTime, string[], double[]>> listDataSeriesChange = new List<Tuple<DateTime, string[], double[]>>();

		//chart中显示的图形
		private List<Annotation> listAnnotation = new List<Annotation>();

		//DS的属性
		ConcurrentDictionary<string, XElement> dicDataSeriesProperties = new ConcurrentDictionary<string, XElement>();

		//策略的报单操作
		List<OrderItem> listOperationLine = new List<OrderItem>();
		private ConcurrentDictionary<OrderItem, int> _dicLotsClosedOfOrder = new ConcurrentDictionary<OrderItem, int>();

		#endregion

		//窗口开启
		private void FormWorkSpace_Load(object sender, EventArgs e)
		{
			//绑定
			int cnt = Math.Min(this.maxShowCandleNum, _stra.D.Count);
			Numeric[] h = new Numeric[cnt];
			Numeric[] l = new Numeric[cnt];
			Numeric[] o = new Numeric[cnt];
			Numeric[] c = new Numeric[cnt];
			Numeric[] v = new Numeric[cnt];
			Numeric[] i = new Numeric[cnt];
			DateTime[] d = new DateTime[cnt];
			for (int j = 0; j < cnt; ++j)
			{
				d[j] = DateTime.ParseExact(_stra.D[cnt - 1 - j].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);
				h[j] = _stra.H[cnt - 1 - j];
				l[j] = _stra.L[cnt - 1 - j];
				o[j] = _stra.O[cnt - 1 - j];
				c[j] = _stra.C[cnt - 1 - j];
				v[j] = _stra.V[cnt - 1 - j];
				i[j] = _stra.I[cnt - 1 - j];
			}

			this.chart1.Series[0].Points.Clear();
			this.chart1.Annotations.Clear();

			this.chart1.Series[0].Points.DataBindXY(d, h, l, o, c);

			for (int j = 0; j < cnt; j++)
			{
				if (j == 0 || _stra.Date[cnt - 1 - j] != _stra.Date[cnt - 1 - j + 1])
					this.chart1.Series[0].Points[j].AxisLabel = d[j].ToString("MM/dd");
				else
					this.chart1.Series[0].Points[j].AxisLabel = d[j].ToString("HH:mm");
			}

			//Y轴显示的小数位数
			var fmt = "F" + (_stra.InstrumentInfo.PriceTick >= 1 ? 0 : _stra.InstrumentInfo.PriceTick.ToString().Split('.')[1].Length - 1);
			var interval = 10 * (double)_stra.InstrumentInfo.PriceTick; //最小跳动
			SetLoadParams(this.chart1, this.chart1.ChartAreas[0], fmt, interval, cnt + 1);
			//foreach (ChartArea area in this.chart1.ChartAreas)
			//	area.AxisY.LabelStyle.Format = "F" + (_stra.InstrumentInfo.PriceTick >= 1 ? 0 : _stra.InstrumentInfo.PriceTick.ToString().Split('.')[1].Length - 1);
			//this.chart1.ChartAreas[0].AxisY.Interval = 100 * (double)_stra.InstrumentInfo.PriceTick; //最小跳动

			//调整显示K线
			//this.chart1.ChartAreas[0].AxisX.Maximum = cnt + 1; //不加此项,重加载数据时显示有问题
			this.chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();

			Zoom(this.chart1);


			this.toolStripLabelInstrument.Text = _stra.InstrumentID;
			this.toolStripLabelInterval.Text = _stra.Interval + " "; //
			switch (_stra.IntervalType)
			{
				//case  EnumIntervalType.Sec:
				//	this.toolStripLabelInterval.Text += "笔";
				//	break;
				case EnumIntervalType.Sec:
					this.toolStripLabelInterval.Text += "秒";
					break;
				case EnumIntervalType.Min:
					this.toolStripLabelInterval.Text += "分";
					break;
				case EnumIntervalType.Hour:
					this.toolStripLabelInterval.Text += "时";
					break;
				case EnumIntervalType.Day:
					this.toolStripLabelInterval.Text += "天";
					break;
				case EnumIntervalType.Week:
					this.toolStripLabelInterval.Text += "周";
					break;
				case EnumIntervalType.Month:
					this.toolStripLabelInterval.Text += "月";
					break;
				default:
					this.toolStripLabelInterval.Text += "年";
					break;
			}
			this.Text = string.Format("{0}[{1}][{2}{3}]", _stra.InstrumentID, this.toolStripLabelInterval.Text, _stra.GetType().Name, _stra.GetParams());

			foreach (var order in _stra.Operations)
			{
				if (order.Date < d[0]) continue;	//只处理显示范围内的信号
				_listOperateArrow.Add(order);
			}
		}


		//窗口关闭
		private void FormWorkSpace_FormClosed(object sender, FormClosedEventArgs e)
		{
		}


		#region 工具栏
		//线型变化
		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			if (this.toolStripButton4.Text == "美线")
			{
				this.chart1.Series[0].ChartType = SeriesChartType.Stock;
				this.toolStripButton4.Text = "蜡烛";
			}
			else
			{
				this.chart1.Series[0].ChartType = SeriesChartType.Candlestick;
				this.toolStripButton4.Text = "美线";
			}
		}

		//放大
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			ZoomOut(this.chart1);
		}

		//缩小
		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			ZoomIn(this.chart1);
		}

		#endregion

		/// <summary>
		/// 关联 this.chart1.GetToolTipText
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
		{
			if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint && e.HitTestResult.Series == this.chart1.Series[0])
			{
				DataPoint dp = (DataPoint)e.HitTestResult.Object;
				DateTime dt = DateTime.FromOADate(dp.XValue);
				int currentBar = _stra.D.Count - 1 - _stra.D.IndexOf(Numeric.Parse(dt.ToString("yyyyMMdd.HHmmss")));
				e.Text = dt.ToString("yyyy/MM/dd[") + dt.ToString("ddd").Substring(1, 1) + "]";
				e.Text += "\r\n时间 = " + dt.TimeOfDay;
				e.Text += "\r\n开 = " + _stra.O[currentBar];
				e.Text += "\r\n高 = " + _stra.H[currentBar];
				e.Text += "\r\n低 = " + _stra.L[currentBar];
				e.Text += "\r\n收 = " + _stra.C[currentBar];
				e.Text += "\r\n量 = " + _stra.V[currentBar];
				e.Text += "\r\n仓 = " + _stra.I[currentBar];


				e.Text += "\r\n[" + _stra.Name + "]";

				//指标提示
				//e.Text += "\r\n[指标]";
				foreach (var a in _stra.Indicators)
				{
					string input = null;
					if (a.Input == _stra.H)
						input = "H";
					else if (a.Input == _stra.L)
						input = "L";
					else if (a.Input == _stra.O)
						input = "O";
					else if (a.Input == _stra.C)
						input = "C";
					else if (a.Input == _stra.V)
						input = "V";
					else if (a.Input == _stra.I)
						input = "I";
					if (input != null)
					{
						e.Text += "\r\n" + a.GetType().Name + "<" + input + ">(";
						foreach (int p in a.Periods)
							e.Text += p + ",";
						e.Text = e.Text.Remove(e.Text.Length - 1);
						e.Text += ")";
						if (a.CustomSeries.Count == 1)
							e.Text += ":" + a.CustomSeries.ElementAt(0).Value[currentBar].ToString("F2");
						else
							foreach (var o in a.CustomSeries)
								e.Text += "\r\n" + o.Key + " : " + o.Value[currentBar].ToString("F2");
					}
				}

				//跨周期
				if (_stra.DicPeriodValue.Count > 0)
					e.Text += "\r\n------ 跨周期 ------";
				foreach (var a in _stra.DicPeriodValue)
				{
					var ldt = _stra.DicPeriodTime[a.Key].LastOrDefault(n => n <= dt);

					int curBar = _stra.DicPeriodTime[a.Key].Count - _stra.DicPeriodTime[a.Key].IndexOf(ldt);
					string i3 = "";
					switch (a.Key.Item3)
					{
						case Strategy.PeriodType.Tick:
							i3 = "笔";
							break;
						case Strategy.PeriodType.Second:
							i3 = "秒";
							break;
						case Strategy.PeriodType.Minute:
							i3 = "分";
							break;
						case Strategy.PeriodType.Hour:
							i3 = "时";
							break;
						case Strategy.PeriodType.Day:
							i3 = "天";
							break;
						case Strategy.PeriodType.Week:
							i3 = "周";
							break;
						case Strategy.PeriodType.Month:
							i3 = "月";
							break;
						case Strategy.PeriodType.Year:
							i3 = "年";
							break;
					}
					e.Text += "\r\n↑" + a.Key.Item1 + " " + a.Key.Item2 + i3 + " : " + a.Value[curBar].ToString("F2");

					//指标提示
					foreach (var i in _stra.Indicators.Where(n => n.Input == a.Value))
					{
						e.Text += "\r\n↑" + i.GetType().Name + "(";
						foreach (int p in i.Periods)
							e.Text += p + ",";
						e.Text = e.Text.Remove(e.Text.Length - 1);
						e.Text += ")";
						if (i.CustomSeries.Count == 1)
							e.Text += ":" + i.CustomSeries.ElementAt(0).Value[curBar].ToString("F2");
						else
							foreach (var o in i.CustomSeries)
								e.Text += "\r\n" + o.Key + " : " + o.Value[curBar].ToString("F2");
					}
				}

				//交易提示
				//foreach (Operation o in v.Value.Operations.Where(n => n.D == dt))
				for (int i = 0; i < _stra.Operations.Count; i++)
				{
					var o = _stra.Operations[i];
					if (o.Date == dt)
					{
						e.Text += $"\r\n{(o.Dir == Direction.Buy ? "买" : "卖")}{(o.Offset == Offset.Open ? "开" : "平")} {o.Lots}@{o.Price:.2}";
						e.Text += $"\r\n[{o.Remark}]";
					}
				}
			}
		}

		#region chart 相关函数

		/// <summary>
		/// 设置加载参数(每次加载新数据时调用1次)
		/// </summary>
		/// <param name="chart1"></param>
		/// <param name="area"></param>
		/// <param name="axisYFmt">area.AxisY.LabelStyle.Format (var fmt = "F" + (_stra.InstrumentInfo.PriceTick >= 1 ? 0 : _stra.InstrumentInfo.PriceTick.ToString().Split('.')[1].Length - 1);</param>
		/// <param name="axisYInterval">area.AxisY.Interval (var interval = 100 * (double)_stra.InstrumentInfo.PriceTick; //最小跳动))</param>
		/// <param name="axisXMax">area.AxisX.Maximum [最大数据量+1] (不加此项,重加载数据时显示有问题)</param>
		public void SetLoadParams(Chart chart1, ChartArea area, string axisYFmt, double axisYInterval, int axisXMax)
		{
			area.AxisY.LabelStyle.Format = axisYFmt;
			area.AxisY.Interval = axisYInterval; //最小跳动
												 //调整显示K线
			area.AxisX.Maximum = axisXMax; //不加此项,重加载数据时显示有问题
		}

		//设置显示区大小
		public void ResetChartArea(Chart chart)
		{
			for (int i = 1; i < chart.ChartAreas.Count; i++)
			{
				bool needShow = chart.Series.Count(n => n.ChartArea == chart.ChartAreas[i].Name) > 0;
				if (needShow && !chart.ChartAreas[i].Visible)
				{
					chart.ChartAreas[0].Position.Height -= 23;
					for (int j = 1; j < i; j++)
					{
						if (chart.ChartAreas[j].Visible)
							chart.ChartAreas[j].Position.Y -= 25;
					}
					chart.ChartAreas[i].Position.Y = 75;
					chart.ChartAreas[i].AxisX.Interval = chart.ChartAreas[0].AxisX.Interval;
					chart.ChartAreas[i].AxisX.IntervalType = chart.ChartAreas[0].AxisX.IntervalType;
					chart.ChartAreas[i].Visible = true;
					chart.ChartAreas[i].AxisX.ScaleView.Zoom(chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum, chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum);
				}
				else if (!needShow && chart.ChartAreas[i].Visible)
				{
					chart.ChartAreas[i].Visible = false;
					chart.ChartAreas[0].Position.Height += 23;
					for (int j = 1; j < i; j++)
					{
						if (chart.ChartAreas[j].Visible)
							chart.ChartAreas[j].Position.Y += 25;
					}
				}
			}
			//重新调整副图高度适应
			ResetAxisY(chart);
		}

		public void ResetAxisY(Chart chart)
		{
			if (chart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
			{
				Series sCur = chart.Series[0];
				int left = Math.Max(0, (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum);
				int right = Math.Min(sCur.Points.Count - 1, (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum);

				double viewTop = double.MinValue, viewButtom = double.MaxValue;

				////调整纵坐标
				//for (int i = left; i <= right; i++)
				//{
				//    viewTop = Math.Max(viewTop, chart.Series[0].Points[i].YValues[0]);
				//    viewButtom = Math.Min(viewButtom, chart.Series[0].Points[i].YValues[chart.Series[0].YValuesPerPoint > 1 ? 1 : 0]);
				//}
				////viewTop += 10 * YValueTick;
				////viewButtom -= 10 * YValueTick;
				//viewTop *= 1.01;
				//viewButtom *= 0.99;
				//chart.ChartAreas[0].AxisY.ScaleView.Zoom(viewButtom, viewTop);

				for (int i = 0; i < chart.ChartAreas.Count; i++)
				{
					viewTop = double.MinValue;
					viewButtom = double.MaxValue;
					if (chart.ChartAreas[i].Visible)
					{
						foreach (Series s in chart.Series.Where(n => n.ChartArea == chart.ChartAreas[i].Name))
						{
							for (int j = left; j <= right; j++)
							{
								viewTop = Math.Max(viewTop, s.Points[j].YValues[0]);
								viewButtom = Math.Min(viewButtom, s.Points[j].YValues[s.YValuesPerPoint > 1 ? 1 : 0]);
							}
						}
						//viewTop += 10 * YValueTick;
						//viewButtom -= 10 * YValueTick;
						//viewTop = (Math.Ceiling(viewTop / 100) + 1) * 100;// *= 1.01;
						//viewButtom = viewButtom / 100 * 100;// *= 0.99;
						//viewTop = viewTop / chart.YValueTick * chart.YValueTick;
						//viewButtom = viewButtom / chart.YValueTick * chart.YValueTick;
						double baseY = Math.Pow(10, Math.Max(0, Math.Ceiling(viewTop).ToString().Length - 3));
						if (baseY == 1)
						{
							viewTop = Math.Ceiling(viewTop);
							viewButtom = Math.Floor(viewButtom);
						}
						else
						{
							viewTop = Math.Ceiling(viewTop / baseY) * baseY + baseY;
							viewButtom = Math.Floor(viewButtom / baseY) * baseY - baseY;
						}
						chart.ChartAreas[i].AxisY.ScaleView.Zoom(viewButtom, viewTop);
					}
				}
			}
		}


		/// <summary>
		/// 缩放
		/// </summary>
		public void Zoom(Chart chart)
		{
			if (chart.Series[0].Points.Count > 200)
			{
				chart.ChartAreas[0].AxisX.ScaleView.Zoom(chart.Series[0].Points.Count - 150, chart.Series[0].Points.Count);
				ResetAxisY(chart);
			}
			else
			{
				chart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
				chart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
			}
		}

		/// <summary>
		/// 放大
		/// </summary>
		public void ZoomOut(Chart chart)
		{
			if (!chart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
				Zoom(chart);

			Series sCur = chart.Series[0];
			int left = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
			int right = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
			if (right - left > 20)
			{
				chart.ChartAreas[0].AxisX.ScaleView.Zoom(left + (right - left) / 3, right);
				ResetAxisY(chart);
			}
		}

		/// <summary>
		/// 缩小
		/// </summary>
		public void ZoomIn(Chart chart)
		{
			if (chart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
			{
				Series sCur = chart.Series[0];
				int left = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
				int right = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
				if (left <= 1 && right >= sCur.Points.Count)
				{
					chart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
					chart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
				}
				else
				{
					if (left == 0)
						chart.ChartAreas[0].AxisX.ScaleView.Zoom(left, right + (right - left) / 3);
					else
						chart.ChartAreas[0].AxisX.ScaleView.Zoom(left - (right - left) / 3, right);
					ResetAxisY(chart);
				}
			}
		}
		#endregion


		//定时刷新: 行情和序列更新待优化
		private void timer1_Tick(object sender, EventArgs e)
		{
			while (this.listQuoteChange.Count > 0)
			{
				if (listQuoteChange[0] != null)
				{
					bool newBar = listQuoteChange[0].Item1;
					DateTime d = listQuoteChange[0].Item2;
					double[] p = listQuoteChange[0].Item3;
					Strategy obj = listQuoteChange[0].Item6;
					string[] names = listQuoteChange[0].Item4;
					double[] values = listQuoteChange[0].Item5;

					if (newBar)
					{
						this.chart1.Series[0].Points.AddXY(d, p[0], p[1], p[2], p[3]);

						for (int i = 1; i < this.chart1.Series.Count; i++)
							this.chart1.Series[i].Points.AddXY(d, this.chart1.Series[i].Points.Last().YValues[0]);
						//界面显示
						this.toolStripLabelDT.Text = d.ToString("yyyy/MM/dd HH:mm");
						this.toolStripLabelO.Text = p[2].ToString("F2");
						this.chart1.ChartAreas[0].AxisX.Maximum++;
						if (this.chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum >= this.chart1.ChartAreas[0].AxisX.Maximum - 2)
							this.chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.Last);
					}

					int cnt = this.chart1.Series[0].Points.Count - 1;
					this.chart1.Series[0].Points[cnt].SetValueY(p[0], p[1], p[2], p[3]);
					for (int i = 0; i < names.Length; i++)
					{
						int sIdx = this.chart1.Series.IndexOf(names[i]);
						if (sIdx >= 0)
							this.chart1.Series[sIdx].Points[cnt].SetValueY(p[i]);
					}
				}
				this.listQuoteChange.RemoveAt(0);
			}

			int last = this.chart1.Series[0].Points.Count - 1;
			if (last >= 0)
			{
				DataPoint dp = this.chart1.Series[0].Points[last];
				this.toolStripLabelH.Text = dp.YValues[0].ToString("F2");
				this.toolStripLabelL.Text = dp.YValues[1].ToString("F2");
				this.toolStripLabelO.Text = dp.YValues[2].ToString("F2");
				this.toolStripLabelC.Text = dp.YValues[3].ToString("F2");
				if (dp.YValues[3] > dp.YValues[2])
					this.toolStripLabelC.ForeColor = Color.IndianRed;
				else if (dp.YValues[3] > dp.YValues[2])
					this.toolStripLabelC.ForeColor = Color.FromArgb(0, 64, 64);
				else
					this.toolStripLabelC.ForeColor = Color.WhiteSmoke;
				this.toolStripLabelUpdateTime.Text = _stra.Tick.UpdateTime + "." + _stra.Tick.UpdateMillisec.ToString().Substring(0, 1);

				ResetChartArea(this.chart1);
			}

			makeAnnotation();
			//放在后面，防止有指令而K线未显示时造成信号不显示
			//if (listAnnotation.Count > 500)
			//{
			//	while (listAnnotation.Count > 500)
			//		listAnnotation.RemoveAt(0);
			//}

			while (listAnnotation.Count > 0)
			{
				//if (listAnnotation[0].X > this.chart1.Series[0].Points.Count - 1)
				//	break;
				listAnnotation[0].Name = "ann" + annotationID++;
				this.chart1.Annotations.Add(listAnnotation[0]);
				listAnnotation.RemoveAt(0);
			}
		}

		//根据模型显示箭头
		private void makeAnnotation()
		{
			while (this._listOperateArrow.Count > 0)
			{
				OrderItem pOperation = this._listOperateArrow[0];
				int idx = this.chart1.Series[0].Points.IndexOf(this.chart1.Series[0].Points.FindByValue(pOperation.Date.ToOADate(), "X"));
				if (idx < 0)
					break;

				ArrowAnnotation arrow = new ArrowAnnotation();
				//arrow.Tag = pOperation.owner;		//标记策略

				arrow.AxisX = this.chart1.ChartAreas[0].AxisX;
				arrow.AxisY = this.chart1.ChartAreas[0].AxisY;
				//arrow.AnchorDataPoint = this.chart1.Series[0].Points.FindByValue(operation.D.ToOADate(), "X");
				arrow.ClipToChartArea = this.chart1.ChartAreas[0].Name;     //防止线出界
				arrow.ArrowSize = 2;// (int)Math.Ceiling(this.chart1.Width / (this.chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum - this.chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum));							//在prepost中进行更改
				arrow.ArrowStyle = ArrowStyle.Simple;//.Tailed;			//双尾箭头
				arrow.LineWidth = 0;                            //不显示边线
				arrow.BackColor = ((pOperation.Dir == Direction.Buy && pOperation.Offset == Offset.Open) || (pOperation.Dir == Direction.Sell && pOperation.Offset != Offset.Open)) ? Color.Red : Color.Blue;   //填充颜色
				arrow.IsSizeAlwaysRelative = true;              //使用绝对值控制下面的高/宽
				arrow.Width = 0;
				arrow.X = idx + 1;      //坐标是从1开始的
				arrow.ToolTip = _stra.Name + ":" +
					(pOperation.Dir == Direction.Buy ? "买" : "卖") + (pOperation.Offset == Offset.Open ? "开" : "平") + pOperation.Lots + "@" + pOperation.Price.ToString("F2");

				arrow.Y = (double)pOperation.Price;
				if (pOperation.Dir == Direction.Sell)
				{
					arrow.Height = -3;// 6 * priceTick;									//在prepost中进行更改:但要保持符号一致
				}
				else
				{
					arrow.Height = 3;// -6 * priceTick;									//在prepost中进行更改:但要保持符号一致
				}
				this._listOperateArrow.RemoveAt(0);

				listAnnotation.Add(arrow);

				listOperationLine.Add(pOperation);
			}

			for (int i = 0; i < this.listOperationLine.Count; i++)
			{
				OrderItem pOperation = this.listOperationLine[i];
				int idx = this.chart1.Series[0].Points.IndexOf(this.chart1.Series[0].Points.FindByValue(pOperation.Date.ToOADate(), "X"));
				if (idx < 0)
					break;

				int lotsClosedClose = _dicLotsClosedOfOrder.GetOrAdd(pOperation, 0);
				if (pOperation.Offset != Offset.Open)
				{
					for (int j = 0; j < i; j++)
					{
						var open = listOperationLine[j];
						int lotsClosedOpen = _dicLotsClosedOfOrder.GetOrAdd(open, 0);
						if (open.Offset == Offset.Open && open.Dir != pOperation.Dir && lotsClosedOpen < open.Lots)
						{
							//找对应的开仓K线
							LineAnnotation line = new LineAnnotation();
							//line.Tag = pOperation.owner;	//标识对应的策略

							//修改已平手数
							int volume = Math.Min(open.Lots - lotsClosedOpen, pOperation.Lots - lotsClosedClose);
							lotsClosedOpen += volume;
							_dicLotsClosedOfOrder[open] = lotsClosedOpen;
							lotsClosedClose += volume;
							_dicLotsClosedOfOrder[pOperation] = lotsClosedClose;

							int idxOpen = this.chart1.Series[0].Points.IndexOf(this.chart1.Series[0].Points.FindByValue(open.Date.ToOADate(), "X"));
							line.IsSizeAlwaysRelative = false;
							line.LineDashStyle = ChartDashStyle.Dash;
							line.AxisX = this.chart1.ChartAreas[0].AxisX;
							line.AxisY = this.chart1.ChartAreas[0].AxisY;
							line.ClipToChartArea = this.chart1.ChartAreas[0].Name;      //防止线出界
																						//line.AnchorDataPoint = this.seriesK.Points[order.BarIndex];
							line.LineWidth = 1;
							line.LineColor = (pOperation.Price - open.Price) * (pOperation.Dir == Direction.Sell ? 1 : -1) > 0 ? Color.IndianRed : Color.MediumBlue;    //盈利:红,亏损:蓝
																																										//line.StartCap = LineAnchorCapStyle.None;//.Arrow;
																																										//line.EndCap = LineAnchorCapStyle.None;//.Arrow;
							line.Width = idx - idxOpen;
							line.X = idxOpen + 1;                       //坐标是从1开始的
							line.Y = (double)open.Price;
							line.Height = (double)(pOperation.Price - open.Price);
							//this.chart1.Annotations.Add(line);
							listAnnotation.Add(line);

							if (lotsClosedOpen == open.Lots) //开仓全部平掉
							{
								listOperationLine.Remove(open);
								j--;
							}
							if (lotsClosedClose == pOperation.Lots) //平仓全部处理
							{
								listOperationLine.Remove(pOperation);
								i--;
								break;
							}
						}
					}
				}
			}
		}

		private void toolStripButtonBlackBack_Click(object sender, EventArgs e)
		{
			Color c = Color.DarkGray;
			foreach (ChartArea area in this.chart1.ChartAreas)
			{
				if (area.BackColor == Color.WhiteSmoke)
					area.BackColor = c;
				else
					area.BackColor = Color.WhiteSmoke;
			}
		}
	}
}
