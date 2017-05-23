using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.Concurrent;
using System.Windows.Forms.DataVisualization.Charting;

namespace HaiFeng
{
	/// <summary>
	/// 
	/// </summary>
	public partial class FormTest : Form
	{
		/// <summary>
		/// 测试
		/// </summary>
		/// <param name="_ws"></param>
		public FormTest(Strategy pStra)
		{
			InitializeComponent();

			dtOperation.Columns.Add("#", typeof(int));
			dtOperation.Columns.Add("多空", typeof(string));
			dtOperation.Columns.Add("开仓时间", typeof(DateTime));
			dtOperation.Columns.Add("开仓价格", typeof(decimal));
			dtOperation.Columns.Add("平仓时间", typeof(DateTime));
			dtOperation.Columns.Add("平仓价格", typeof(decimal));
			dtOperation.Columns.Add("手数", typeof(int));
			dtOperation.Columns.Add("手续费", typeof(decimal));
			dtOperation.Columns.Add("净利", typeof(decimal));
			dtOperation.Columns.Add("净利合计", typeof(decimal));
			dtOperation.Columns.Add("收益率", typeof(decimal));
			this.dataGridViewDetail.DataSource = dtOperation;

			this.dataGridViewDetail.Columns["#"].DefaultCellStyle.Format = "N0";
			this.dataGridViewDetail.Columns["多空"].DefaultCellStyle.Format = "F2";
			this.dataGridViewDetail.Columns["开仓时间"].DefaultCellStyle.Format = "g";
			this.dataGridViewDetail.Columns["开仓价格"].DefaultCellStyle.Format = "F2";
			this.dataGridViewDetail.Columns["平仓时间"].DefaultCellStyle.Format = "g";
			this.dataGridViewDetail.Columns["平仓价格"].DefaultCellStyle.Format = "F2";
			//this.dataGridViewDetail.Columns["手数"]
			this.dataGridViewDetail.Columns["手续费"].DefaultCellStyle.Format = "F2";
			this.dataGridViewDetail.Columns["净利"].DefaultCellStyle.Format = "F2";
			this.dataGridViewDetail.Columns["净利合计"].DefaultCellStyle.Format = "F2";
			this.dataGridViewDetail.Columns["收益率"].DefaultCellStyle.Format = "F2";
			this.dataGridViewDetail.Columns["多空"].DefaultCellStyle.Format = "F2";

			this.dataGridViewDetail.Columns["多空"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.dataGridViewDetail.Columns["开仓时间"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.dataGridViewDetail.Columns["开仓价格"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewDetail.Columns["平仓时间"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.dataGridViewDetail.Columns["平仓价格"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewDetail.Columns["手数"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.dataGridViewDetail.Columns["手续费"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewDetail.Columns["净利"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewDetail.Columns["净利合计"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			this.dataGridViewDetail.Columns["收益率"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			this.dataGridViewDetail.Columns["收益率"].Visible = false;
			//ws = _ws;
			_stra = pStra;
		}

		private DataTable dtOperation = new DataTable("交易记录");
		//private DataGridViewRow ws = null;
		private Strategy _stra = null;
		List<decimal> equity = new List<decimal>();

		/// <summary>
		/// 已在最后增加平仓指令
		/// </summary>
		List<Tuple<string, List<OrderItem>>> listCopy = new List<Tuple<string, List<OrderItem>>>();

		/// <summary>
		/// 品种,开费率,开费,平费率,平费,平今费率,平今费
		/// </summary>
		//internal Tuple<decimal, decimal, decimal, decimal, decimal, decimal> commissionRate = null;

		private void FormTest_Load(object sender, EventArgs e)
		{
			if (_stra == null)
			{
				MessageBox.Show("工作区无策略加载!");
				this.Close();
				return;
			}

			AdjustRate();
			this.chartDB1.ChartAreas[0].AxisY.Interval = 100;

			var v = _stra;
			{
				//复制所有交易,并在最后增加平仓
				List<OrderItem> operationCopy = new List<OrderItem>();
				if (v.Operations.Count > 0)
				{
					for (int i = 0; i < v.Operations.Count; i++)
					{
						operationCopy.Add(v.Operations[i]);
					}
					if (v.Operations.Last().Offset == Offset.Open)
					{
						OrderItem tmp = new OrderItem();
						if (v.PositionLong > 0)
						{
							tmp.Date = DateTime.ParseExact(v.D[0].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);
							tmp.Dir = Direction.Sell;
							tmp.Lots = v.PositionLong;
							tmp.Offset = Offset.Close;
							tmp.Price = v.C[0];
							operationCopy.Add(tmp);
						}
						if (v.PositionShort > 0)
						{
							tmp.Date = DateTime.ParseExact(v.D[0].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);
							tmp.Dir = Direction.Buy;
							tmp.Lots = v.PositionShort;
							tmp.Offset = Offset.Close;
							tmp.Price = v.C[0];
							operationCopy.Add(tmp);
						}
					}
				}
				listCopy.Add(new Tuple<string, List<OrderItem>>(v.Name, operationCopy));
				this.toolStripComboBoxStrategy.Items.Add(v.Name);
			}
			this.toolStripComboBoxStrategy.SelectedIndex = 0;
			this.comboBoxChartItem.SelectedIndex = 0;
		}


		private void FormTest_FormClosed(object sender, FormClosedEventArgs e)
		{
			dtRate.WriteXml(dtRate.TableName);
		}


		//策略选择
		private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			dtOperation.Rows.Clear();
			this.dataGridViewdReport.Rows.Clear();
			foreach (Series s in this.chartDB1.Series)
				s.Points.Clear();

			string product = new string(_stra.InstrumentID.TakeWhile(char.IsLetter).ToArray());
			var procInfo = _stra.Datas[0].InstrumentInfo;
			DataRow dr = dtRate.Rows.Find(product);
			var crate = new Tuple<decimal, decimal, decimal, decimal, decimal>((decimal)dr[1], (decimal)dr[2], (decimal)dr[3], (decimal)dr[4], (decimal)dr[5]);// dicRate[product];

			List<OrderItem> operationCopy = new List<OrderItem>();
			//for (int i = 0; i < this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2.Count; i++)
			//{
			//    OrderItem operate = new OrderItem();
			//    operate.D = this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2[i].D;
			//    operate.Dir = this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2[i].Dir;
			//    operate.Lots = this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2[i].Lots;
			//    operate.LotsClosed = this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2[i].LotsClosed;
			//    operate.Offset = this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2[i].Offset;
			//    operate.Price = this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2[i].Price;
			//    operationCopy.Add(operate);
			//}
			foreach (var v in this.listCopy[this.toolStripComboBoxStrategy.SelectedIndex].Item2)
			{
				OrderItem operate = new OrderItem();
				operate.Date = v.Date;
				operate.Dir = v.Dir;
				operate.Lots = v.Lots;
				//operate.LotsClosed = v.LotsClosed;
				operate.Offset = v.Offset;
				operate.Price = v.Price;
				operationCopy.Add(operate);
			}
			if (operationCopy.Count == 0)
				return;

			int 最大连续盈利次数 = 0, 最大连续亏损次数 = 0, 最大连续盈利次数多 = 0, 最大连续亏损次数多 = 0, 最大连续盈利次数空 = 0, 最大连续亏损次数空 = 0;
			int p最大连续盈利次数 = 0, p最大连续亏损次数 = 0, p最大连续盈利次数多 = 0, p最大连续亏损次数多 = 0, p最大连续盈利次数空 = 0, p最大连续亏损次数空 = 0;

			//生成动态盈亏
			decimal avgLong = 0, avgShort = 0;
			int pLong = 0, pShort = 0;
			decimal maxMarginLong = 0, maxMarginShort = 0;
			decimal eq = 0, maxEquity = 0, maxReturn = 0, maxRate = 0;
			DateTime dtMaxReturn = DateTime.MinValue;
			int idxOperation = 0;
			for (int i = _stra.C.Count - 1; i >= 0; --i)
			{
				decimal eqLong = 0, eqShort = 0;
				eqLong = (_stra.C[i] - avgLong) * pLong; //持有多仓浮盈
				eqShort = (avgShort - _stra.C[i]) * pShort; //持仓空仓浮盈

				while (idxOperation < _stra.Operations.Count && _stra.Operations[idxOperation].Date == DateTime.ParseExact(_stra.D[i].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null))
				{
					var oi = _stra.Operations[idxOperation];
					if (oi.Dir == Direction.Buy)
					{
						if (oi.Offset == Offset.Open)
						{
							avgLong = pLong == 0 ? oi.Price : ((avgLong * pLong + oi.Price * oi.Lots) / (pLong + oi.Lots));
							pLong += oi.Lots;
						}
						else
						{
							eq += eqShort = (avgShort - oi.Price) * oi.Lots * procInfo.VolumeMultiple; //平仓盈亏
							pShort -= oi.Lots;
						}
					}
					else
					{
						if (oi.Offset == Offset.Open)
						{
							avgShort = pShort == 0 ? oi.Price : ((avgShort * pShort + oi.Price * oi.Lots) / (pShort + oi.Lots));
							pShort += oi.Lots;
						}
						else
						{
							eq += eqLong = (oi.Price - avgLong) * oi.Lots * procInfo.VolumeMultiple; //平仓盈亏
							pLong -= oi.Lots;
						}
					}
					++idxOperation;
				}

				decimal eqCur = eq + eqLong + eqShort;

				maxMarginLong = Math.Max(maxMarginLong, _stra.H[i] * pLong * _stra.InstrumentInfo.VolumeMultiple * crate.Item4);
				maxMarginShort = Math.Max(maxMarginShort, _stra.H[i] * pShort * _stra.InstrumentInfo.VolumeMultiple * crate.Item5);
				maxEquity = Math.Max(maxEquity, eqCur);
				if (maxEquity - eqCur > maxReturn) //超过之前的最大回撤
				{
					maxReturn = Math.Max(maxReturn, maxEquity - eqCur);
					dtMaxReturn = DateTime.ParseExact(_stra.D[i].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);
					maxRate = maxEquity == 0 ? 0 : (maxReturn / maxEquity);
				}
				equity.Add(eqCur);
			}

			#region 生成交易记录
			for (int i = 0; i < operationCopy.Count; i++)
			{
				if (operationCopy[i].Offset != Offset.Open)
				{
					//查找对应的开仓
					if (operationCopy[i].Dir == Direction.Sell) //SP
					{
						for (int iOpen = 0; iOpen < operationCopy.Count; iOpen++)
						{
							if (operationCopy[iOpen].Offset == Offset.Open && operationCopy[iOpen].Dir == Direction.Buy)
							{
								int closeLots = Math.Min(operationCopy[iOpen].Lots, operationCopy[i].Lots);
								operationCopy[iOpen].Lots -= closeLots;
								operationCopy[i].Lots -= closeLots;
								DataRow drNew = this.dtOperation.NewRow();
								drNew["#"] = this.dtOperation.Rows.Count + 1;
								drNew["多空"] = "多";
								drNew["开仓时间"] = operationCopy[iOpen].Date;
								drNew["开仓价格"] = operationCopy[iOpen].Price;
								drNew["平仓时间"] = operationCopy[i].Date;
								drNew["平仓价格"] = operationCopy[i].Price;
								drNew["手数"] = closeLots;
								drNew["手续费"] = closeLots * ((crate.Item1 > 1 ? crate.Item1 : (operationCopy[iOpen].Price * crate.Item1)) + (crate.Item2 > 1 ? crate.Item2 : (operationCopy[i].Price * crate.Item2)));
								drNew["净利"] = (operationCopy[i].Price - operationCopy[iOpen].Price) * closeLots * procInfo.VolumeMultiple - (decimal)drNew["手续费"];

								DataRow drLast = this.dtOperation.AsEnumerable().LastOrDefault();
								if (drLast == null)
									drNew["净利合计"] = drNew["净利"];
								else
									drNew["净利合计"] = (decimal)drLast["净利合计"] + (decimal)drNew["净利"];
								this.dtOperation.Rows.Add(drNew);
								if ((decimal)drNew["净利"] > 0)
								{
									p最大连续盈利次数++;
									p最大连续盈利次数多++;
									p最大连续亏损次数 = p最大连续亏损次数多 = 0;

									最大连续盈利次数多 = Math.Max(最大连续盈利次数多, p最大连续盈利次数多);
									最大连续盈利次数 = Math.Max(最大连续盈利次数, 最大连续盈利次数多);
								}
								else
								{
									p最大连续亏损次数++;
									p最大连续亏损次数多++;
									p最大连续盈利次数 = p最大连续盈利次数多 = 0;

									最大连续亏损次数多 = Math.Max(最大连续亏损次数多, p最大连续亏损次数多);
									最大连续亏损次数 = Math.Max(最大连续亏损次数, 最大连续亏损次数多);
								}

								if (operationCopy[iOpen].Lots == 0)
								{
									operationCopy.RemoveAt(iOpen);
									iOpen--;
									i--;
								}
								if (operationCopy[i].Lots == 0)
								{
									operationCopy.RemoveAt(i);
									iOpen--;
									i--;
									break;
								}
							}
						}
					}
					else
					{
						for (int iOpen = 0; iOpen < operationCopy.Count; iOpen++)   //BP
						{
							if (operationCopy[iOpen].Offset == Offset.Open && operationCopy[iOpen].Dir == Direction.Sell)
							{
								int closeLots = Math.Min(operationCopy[iOpen].Lots, operationCopy[i].Lots);
								operationCopy[iOpen].Lots -= closeLots;
								operationCopy[i].Lots -= closeLots;
								DataRow drNew = this.dtOperation.NewRow();
								drNew["#"] = this.dtOperation.Rows.Count + 1;
								drNew["多空"] = "空";
								drNew["开仓时间"] = operationCopy[iOpen].Date;
								drNew["开仓价格"] = operationCopy[iOpen].Price;
								drNew["平仓时间"] = operationCopy[i].Date;
								drNew["平仓价格"] = operationCopy[i].Price;
								drNew["手数"] = closeLots;
								drNew["手续费"] = closeLots * ((crate.Item1 > 1 ? crate.Item1 : (operationCopy[iOpen].Price * crate.Item1)) + (crate.Item2 > 1 ? crate.Item2 : (operationCopy[i].Price * crate.Item2)));
								drNew["净利"] = -(operationCopy[i].Price - operationCopy[iOpen].Price) * closeLots * procInfo.VolumeMultiple - (decimal)drNew["手续费"];
								DataRow drLast = this.dtOperation.AsEnumerable().LastOrDefault();
								if (drLast == null)
									drNew["净利合计"] = drNew["净利"];
								else
									drNew["净利合计"] = (decimal)drLast["净利合计"] + (decimal)drNew["净利"];
								this.dtOperation.Rows.Add(drNew);
								if ((decimal)drNew["净利"] > 0)
								{
									p最大连续盈利次数++;
									p最大连续盈利次数空++;
									p最大连续亏损次数 = p最大连续亏损次数空 = 0;

									最大连续盈利次数空 = Math.Max(最大连续盈利次数空, p最大连续盈利次数空);
									最大连续盈利次数 = Math.Max(最大连续盈利次数, 最大连续盈利次数空);
								}
								else
								{
									p最大连续亏损次数++;
									p最大连续亏损次数空++;
									p最大连续盈利次数 = p最大连续盈利次数空 = 0;

									最大连续亏损次数空 = Math.Max(最大连续亏损次数空, p最大连续亏损次数空);
									最大连续亏损次数 = Math.Max(最大连续亏损次数, 最大连续亏损次数空);
								}
								if (operationCopy[iOpen].Lots == 0)
								{
									operationCopy.RemoveAt(iOpen);
									iOpen--;
									i--;
								}
								if (operationCopy[i].Lots == 0)
								{
									operationCopy.RemoveAt(i);
									iOpen--;
									i--;
									break;
								}
							}
						}
					}
				}
			}
			#endregion

			#region 生成动态盈亏表
			//this.dataGridViewdEquity.Rows.Clear();
			//for (int i = 0; i < ws._dt.Count; i++)
			//{
			//	this.dataGridViewdEquity.Rows.Insert(0, 1);
			//	this.dataGridViewdEquity.Rows[0].Cells["bar"].Value = i;
			//	this.dataGridViewdEquity.Rows[0].Cells["时间"].Value = ws._dt[ws._dt.Count - 1 - i];
			//	this.dataGridViewdEquity.Rows[0].Cells["盈亏"].Value = this.dicEquity[this.toolStripComboBoxStrategy.Text][ws._dt.Count - 1 - i];
			//}
			#endregion

			#region 综合报告
			var drsLong = this.dtOperation.AsEnumerable().Where(n => (string)n["多空"] == "多");
			var drsShort = this.dtOperation.AsEnumerable().Where(n => (string)n["多空"] == "空");

			int idx = this.dataGridViewdReport.Rows.Add();
			DataGridViewRow row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "净利润";
			decimal 净利润 = (decimal)this.dtOperation.Rows[this.dtOperation.Rows.Count - 1]["净利合计"];
			row.Cells[1].Value = 净利润;
			decimal 净利润多 = drsLong.Count() == 0 ? 0 : drsLong.Sum(n => (decimal)n["净利"]);
			row.Cells[2].Value = 净利润多;
			decimal 净利润空 = drsShort.Count() == 0 ? 0 : drsShort.Sum(n => (decimal)n["净利"]);
			row.Cells[3].Value = 净利润空;

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "总盈利";
			decimal 总盈利多 = drsLong.Count() == 0 ? 0 : drsLong.Where(n => (decimal)n["净利"] > 0).Sum(n => (decimal)n["净利"]);
			row.Cells[2].Value = 总盈利多;
			decimal 总盈利空 = drsShort.Count() == 0 ? 0 : drsShort.Where(n => (decimal)n["净利"] > 0).Sum(n => (decimal)n["净利"]);
			row.Cells[3].Value = 总盈利空;
			row.Cells[1].Value = (decimal)row.Cells[2].Value + (decimal)row.Cells[3].Value;

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "总亏损";
			decimal 总亏损多 = drsLong.Count() == 0 ? 0 : drsLong.Where(n => (decimal)n["净利"] < 0).Sum(n => (decimal)n["净利"]);
			row.Cells[2].Value = 总亏损多;
			decimal 总亏损空 = drsShort.Count() == 0 ? 0 : drsShort.Where(n => (decimal)n["净利"] < 0).Sum(n => (decimal)n["净利"]);
			row.Cells[3].Value = 总亏损空;
			row.Cells[1].Value = (decimal)row.Cells[2].Value + (decimal)row.Cells[3].Value;

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "总盈利/总亏损";
			row.Cells[2].Value = Math.Abs(总盈利多 / 总亏损多);
			row.Cells[3].Value = Math.Abs(总盈利空 / 总亏损空);
			row.Cells[1].Value = Math.Abs((总盈利多 + 总盈利空) / (总亏损多 + 总亏损空));

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "交易手数";
			int 手数多 = drsLong.Count() == 0 ? 0 : drsLong.Sum(n => (int)n["手数"]);
			row.Cells[2].Value = 手数多;
			int 手数空 = drsShort.Count() == 0 ? 0 : drsShort.Sum(n => (int)n["手数"]);
			row.Cells[3].Value = 手数空;
			int 手数 = 手数多 + 手数空;
			row.Cells[1].Value = 手数;

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "盈利比率";
			int 盈利手数多 = drsLong.Count() == 0 ? 0 : drsLong.Where(n => (decimal)n["净利"] > 0).Sum(n => (int)n["手数"]);
			row.Cells[2].Value = 手数多 == 0 ? 0 : (decimal)盈利手数多 / 手数多;
			int 盈利手数空 = drsShort.Count() == 0 ? 0 : drsShort.Where(n => (decimal)n["净利"] > 0).Sum(n => (int)n["手数"]);
			row.Cells[3].Value = 手数空 == 0 ? 0 : (decimal)盈利手数空 / 手数空;
			row.Cells[1].Value = (decimal)(盈利手数多 + 盈利手数空) / (手数多 + 手数空);
			row.Cells[1].Style.Format = row.Cells[2].Style.Format = row.Cells[3].Style.Format = "P2";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "盈利手数";
			row.Cells[2].Value = 盈利手数多;
			row.Cells[3].Value = 盈利手数空;
			row.Cells[1].Value = 盈利手数多 + 盈利手数空;
			row.Cells[1].Style.Format = row.Cells[2].Style.Format = row.Cells[3].Style.Format = "N0";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "亏损手数";
			int 亏损手数多 = drsLong.Count() == 0 ? 0 : drsLong.Where(n => (decimal)n["净利"] < 0).Sum(n => (int)n["手数"]);
			row.Cells[2].Value = 亏损手数多;
			int 亏损手数空 = drsShort.Count() == 0 ? 0 : drsShort.Where(n => (decimal)n["净利"] < 0).Sum(n => (int)n["手数"]);
			row.Cells[3].Value = 亏损手数空;
			row.Cells[1].Value = (int)row.Cells[2].Value + (int)row.Cells[3].Value;
			row.Cells[1].Style.Format = row.Cells[2].Style.Format = row.Cells[3].Style.Format = "N0";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "持平手数";
			row.Cells[2].Value = 手数多 - 盈利手数多 - 亏损手数多;
			row.Cells[3].Value = 手数空 - 盈利手数空 - 亏损手数空;
			row.Cells[1].Value = (int)row.Cells[2].Value + (int)row.Cells[3].Value;
			row.Cells[1].Style.Format = row.Cells[2].Style.Format = row.Cells[3].Style.Format = "N0";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "平均利润";
			row.Cells[2].Value = 手数多 == 0 ? 0 : 净利润多 / 手数多;
			row.Cells[3].Value = 手数空 == 0 ? 0 : 净利润空 / 手数空;
			row.Cells[1].Value = 净利润 / 手数;

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "平均盈利";
			row.Cells[2].Value = 手数多 == 0 ? 0 : 总盈利多 / 盈利手数多;
			row.Cells[3].Value = 手数空 == 0 ? 0 : 总盈利空 / 盈利手数空;
			row.Cells[1].Value = (总盈利多 + 总盈利空) / (盈利手数多 + 盈利手数空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "平均亏损";
			row.Cells[2].Value = 手数多 == 0 ? 0 : 总亏损多 / 亏损手数多;
			row.Cells[3].Value = 手数空 == 0 ? 0 : 总亏损空 / 亏损手数空;
			row.Cells[1].Value = (总亏损多 + 总亏损空) / (亏损手数多 + 亏损手数空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "平均盈利/平均亏损";
			row.Cells[2].Value = 手数多 == 0 ? 0 : (总盈利多 / 盈利手数多) / (总亏损多 / 亏损手数多);
			row.Cells[3].Value = 手数空 == 0 ? 0 : (总盈利空 / 盈利手数空) / (总亏损空 / 亏损手数空);
			row.Cells[1].Value = ((总盈利多 + 总盈利空) / (盈利手数多 + 盈利手数空)) / ((总亏损多 + 总亏损空) / (亏损手数多 + 亏损手数空));

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大盈利";
			decimal 最大盈利多 = drsLong.Count() == 0 ? 0 : drsLong.Max(n => (decimal)n["净利"]);
			最大盈利多 = Math.Max(0, 最大盈利多);
			row.Cells[2].Value = 最大盈利多;
			decimal 最大盈利空 = drsShort.Count() == 0 ? 0 : drsShort.Max(n => (decimal)n["净利"]);
			最大盈利空 = Math.Max(0, 最大盈利空);
			row.Cells[3].Value = 最大盈利空;
			row.Cells[1].Value = Math.Max(最大盈利多, 最大盈利空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大亏损";
			decimal 最大亏损多 = drsLong.Count() == 0 ? 0 : drsLong.Min(n => (decimal)n["净利"]);
			row.Cells[2].Value = 最大亏损多;
			decimal 最大亏损空 = drsShort.Count() == 0 ? 0 : drsShort.Min(n => (decimal)n["净利"]);
			row.Cells[3].Value = 最大亏损空;
			row.Cells[1].Value = Math.Min(最大亏损多, 最大亏损空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大盈利/总盈利";
			row.Cells[2].Value = 最大盈利多 / 总盈利多;
			row.Cells[3].Value = 最大盈利空 / 总盈利空;
			row.Cells[1].Value = Math.Max(最大盈利多, 最大盈利空) / (总盈利多 + 总盈利空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大亏损/总亏损";
			row.Cells[2].Value = 最大亏损多 / 总亏损多;
			row.Cells[3].Value = 最大亏损空 / 总亏损空;
			row.Cells[1].Value = Math.Min(最大亏损多, 最大亏损空) / (总亏损多 + 总亏损空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "净利润/最大亏损";
			row.Cells[2].Value = 净利润多 / -最大亏损多;
			row.Cells[3].Value = 净利润空 / -最大亏损空;
			row.Cells[1].Value = 净利润 / -Math.Min(最大亏损多, 最大亏损空);

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大连续盈利次数";
			row.Cells[2].Value = 最大连续盈利次数多;
			row.Cells[3].Value = 最大连续盈利次数空;
			row.Cells[1].Value = 最大连续盈利次数;
			row.Cells[1].Style.Format = row.Cells[2].Style.Format = row.Cells[3].Style.Format = "N0";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大连续亏损次数";
			row.Cells[2].Value = 最大连续亏损次数多;
			row.Cells[3].Value = 最大连续亏损次数空;
			row.Cells[1].Value = 最大连续亏损次数;
			row.Cells[1].Style.Format = row.Cells[2].Style.Format = row.Cells[3].Style.Format = "N0";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大保证金占用";
			row.Cells[2].Value = maxMarginLong;// 0;// _stra.MaxMarginLong * crate.Item4;
			row.Cells[3].Value = maxMarginShort;// 0;// ws.DicStrategys[this.toolStripComboBoxStrategy.Text].MaxMarginShort * crate.Item5;
			decimal 最大保证金占用 = Math.Max(maxMarginLong, maxMarginShort);// Math.Max(decimal.Parse(row.Cells[2].Value.ToString()), decimal.Parse(row.Cells[3].Value.ToString()));
			row.Cells[1].Value = 最大保证金占用;

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "佣金合计";
			row.Cells[2].Value = drsLong.Count() == 0 ? 0 : drsLong.Sum(n => (decimal)n["手续费"]);
			row.Cells[3].Value = drsShort.Count() == 0 ? 0 : drsShort.Sum(n => (decimal)n["手续费"]);
			row.Cells[1].Value = (decimal)row.Cells[2].Value + (decimal)row.Cells[3].Value;

			TimeSpan diff = DateTime.ParseExact(_stra.D[0].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null) - DateTime.ParseExact(_stra.D.First().ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);// (ws.DicStrategys[this.toolStripComboBoxStrategy.Text].D[0] - ws.DicStrategys[this.toolStripComboBoxStrategy.Text].D[ws.DicStrategys[this.toolStripComboBoxStrategy.Text].CurrentBar]);
			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "收益率(最大保证金)";
			row.Cells[1].Value = 净利润 / 最大保证金占用;
			row.Cells[1].Style.Format = "P2";
			row.Cells[2].Value = "";
			row.Cells[3].Value = "";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "年度收益率（最大保证金）";
			row.Cells[1].Value = 净利润 / diff.Days * 360 / 最大保证金占用;
			row.Cells[1].Style.Format = "P2";
			row.Cells[2].Value = "";
			row.Cells[3].Value = "";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "月度平均盈利";
			row.Cells[1].Value = 净利润 / diff.Days * 30;
			row.Cells[2].Value = "";
			row.Cells[3].Value = "";

			//for (int i = ws.dt.Count - 1; i >= 0; i--)
			//{
			//	maxEquity = Math.Max(maxEquity, ws.DicStrategys[this.toolStripComboBoxStrategy.Text].Series["@equity"][i]);
			//	if (maxEquity - ws.DicStrategys[this.toolStripComboBoxStrategy.Text].Series["@equity"][i] > maxReturn)
			//	{
			//		maxReturn = maxEquity - ws.DicStrategys[this.toolStripComboBoxStrategy.Text].Series["@equity"][i];
			//		dtMaxReturn = ws.dt[i];
			//		maxRate = maxReturn / maxEquity;
			//	}
			//}
			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "最大回撤";
			row.Cells[1].Value = -maxReturn;
			row.Cells[2].Value = "";
			row.Cells[3].Value = "";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "发生时间";
			row.Cells[1].Value = dtMaxReturn;
			row.Cells[1].Style.Format = "G";
			row.Cells[2].Value = "";
			row.Cells[3].Value = "";

			idx = this.dataGridViewdReport.Rows.Add();
			row = this.dataGridViewdReport.Rows[idx];
			row.Cells[0].Value = "回撤/前期高点";
			row.Cells[1].Value = maxRate;
			row.Cells[1].Style.Format = "P2";
			row.Cells[2].Value = "";
			row.Cells[3].Value = "";

			//设置单元格格式
			foreach (DataGridViewRow r in this.dataGridViewdReport.Rows)
			{
				decimal tmp = 0;
				if (decimal.TryParse(r.Cells[1].Value.ToString(), out tmp) && tmp < 0)
					r.Cells[1].Style.SelectionForeColor = r.Cells[1].Style.ForeColor = Color.Red;
				if (decimal.TryParse(r.Cells[2].Value.ToString(), out tmp) && tmp < 0)
					r.Cells[2].Style.SelectionForeColor = r.Cells[2].Style.ForeColor = Color.Red;
				if (decimal.TryParse(r.Cells[3].Value.ToString(), out tmp) && tmp < 0)
					r.Cells[3].Style.SelectionForeColor = r.Cells[3].Style.ForeColor = Color.Red;
			}
			#endregion
			comboBox1_SelectedIndexChanged(null, null);
		}

		//选择图表类型
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.chartDB1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
			this.chartDB1.ChartAreas[0].AxisY.ScaleView.ZoomReset();

			for (int i = 1; i < this.chartDB1.Series.Count; i++)
				this.chartDB1.Series.RemoveAt(i);

			this.chartDB1.Series[0].Points.Clear();
			this.chartDB1.Annotations.Clear();
			chartDB1.Series[0].ChartType = SeriesChartType.Line;
			this.chartDB1.Series[0].MarkerStyle = MarkerStyle.None;

			if (this.comboBoxChartItem.Text == "平仓权益")
			{
				if (dtOperation.Rows.Count > 300)
					chartDB1.Series[0].ChartType = SeriesChartType.FastLine;
				this.chartDB1.Series[0].MarkerStyle = MarkerStyle.Circle;
				//for (int i = 0; i < dtOperation.Rows.Count; i++)
				foreach (DataRow dr in dtOperation.Rows)
				{
					//DataRow dr = dtOperation.Rows[i];
					this.chartDB1.Series[0].Points.AddXY((DateTime)dr["平仓时间"], (decimal)dr["净利合计"]);
				}
			}
			else if (this.comboBoxChartItem.Text == "动态权益")
			{
				if (_stra.D.Count > 300)
					chartDB1.Series[0].ChartType = SeriesChartType.FastLine;
				DateTime[] ds = new DateTime[_stra.D.Count];

				for (int i = 0; i < ds.Length; ++i)
				{
					ds[i] = DateTime.ParseExact(_stra.D[ds.Length - 1 - i].ToString("00000000.000000"), "yyyyMMdd.HHmmss", null);
				}
				this.chartDB1.Series[0].Points.DataBindXY(ds, equity);// this.dicEquity[this.toolStripComboBoxStrategy.Text]);
			}
			else if (this.comboBoxChartItem.Text == "月度盈亏")
			{
				//for (int i = 0; i < dtOperation.Rows.Count; i++)
				//{
				//DataRow dr = dtOperation.Rows[i];
				//this.chartDB1.Series[0].Points.AddXY((DateTime)dr["平仓时间"], (decimal)dr["净利"]);
				//}
				List<decimal> y = new List<decimal>();
				List<DateTime> x = new List<DateTime>();
				foreach (DataRow dr in dtOperation.Rows)
				{
					DateTime dtTmp = (DateTime)dr["平仓时间"];
					int current = y.Count - 1;
					if (current < 0 || x[current].Month != dtTmp.Month)
					{
						x.Add(new DateTime(dtTmp.Year, dtTmp.Month, 1));
						y.Add((decimal)dr["净利"]);
					}
					else
					{
						y[current] += (decimal)dr["净利"];
					}
				}
				chartDB1.Series[0].Points.DataBindXY(x, y);
				//chartDB1.DataManipulator.Group("SUM", 1, IntervalType.Months, this.chartDB1.Series[0]);
				chartDB1.Series[0].ChartType = SeriesChartType.Column;
				for (int i = 0; i < chartDB1.Series[0].Points.Count; i++)
				{
					chartDB1.Series[0].Points[i].AxisLabel = DateTime.FromOADate(chartDB1.Series[0].Points[i].XValue).ToString("yyyy年MM月");
					if (chartDB1.Series[0].Points[i].YValues[0] > 0)
						chartDB1.Series[0].Points[i].Color = Color.Red;
					else
						chartDB1.Series[0].Points[i].Color = Color.Green;
				}
			}
			else if (this.comboBoxChartItem.Text == "盈亏分布")
			{
				this.checkBoxMA.Checked = false;

				for (int i = 0; i < dtOperation.Rows.Count; i++)
				{
					DataRow dr = dtOperation.Rows[i];
					this.chartDB1.Series[0].Points.AddXY((DateTime)dr["平仓时间"], (decimal)dr["净利"]);
				}
				HorizontalLineAnnotation hl = new HorizontalLineAnnotation();
				hl.Y = chartDB1.DataManipulator.Statistics.Mean(this.chartDB1.Series[0].Name);
				hl.X = 0;
				hl.AxisX = this.chartDB1.ChartAreas[0].AxisX;
				hl.AxisY = this.chartDB1.ChartAreas[0].AxisY;
				hl.ClipToChartArea = this.chartDB1.ChartAreas[0].Name;
				hl.Width = 100;
				hl.LineColor = Color.IndianRed;
				hl.LineWidth = 2;
				hl.ToolTip = hl.Y.ToString("N2");
				this.chartDB1.Annotations.Add(hl);
				this.chartDB1.Series[0].ChartType = SeriesChartType.Point;
			}

			if (this.checkBoxMA.Checked)
				checkBoxMA_CheckedChanged(null, null);
		}

		//手续费设置
		DataTable dtRate = new DataTable("费率(测试)");


		/// <summary>
		/// 调整手续费:重新计算资金曲线
		/// </summary>
		public void AdjustRate()
		{
			//foreach (var model in ws.DicStrategys)
			//	equityReset(model.Value);

			if (dtRate.Columns.Count == 0)
			{
				dtRate.Columns.Add("品种", typeof(string));
				dtRate.Columns.Add("开仓费率", typeof(decimal));
				dtRate.Columns.Add("平仓费率", typeof(decimal));
				dtRate.Columns.Add("平今费率", typeof(decimal));
				dtRate.Columns.Add("保证金多", typeof(decimal));
				dtRate.Columns.Add("保证金空", typeof(decimal));
				dtRate.PrimaryKey = new[] { dtRate.Columns["品种"] };
			}

			dtRate.Rows.Clear();
			if (File.Exists(dtRate.TableName))
				dtRate.ReadXml(dtRate.TableName);

			if (dtRate.Rows.Find(_stra.InstrumentInfo.ProductID) == null)
			{
				decimal dFee = _stra.InstrumentInfo.PriceTick * _stra.InstrumentInfo.VolumeMultiple * 2;
				dtRate.Rows.Add(_stra.InstrumentInfo.ProductID, dFee, dFee, 0, 0.15, 0.15);
			}

			this.dataGridViewdRate.DataSource = dtRate;
			this.dataGridViewdRate.Columns["品种"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

		}

		ConcurrentDictionary<string, List<decimal>> dicEquity = new ConcurrentDictionary<string, List<decimal>>();
		//手续费改变后,调此函数即可刷新盈亏曲线
		//void equityReset(Strategy model)
		//{
		//	//重新计算动态权益曲线
		//	List<decimal> equity = new List<decimal>(); //未去除手续费
		//	dicEquity.TryAdd(model.Name, equity);

		//	Tuple<decimal, decimal, decimal, decimal, decimal> rate = dicRate[ws.InstrumentField.ProductID];
		//	decimal feeOpen = rate.Item1;
		//	decimal feeClose = rate.Item2;
		//	decimal sumOpen = 0, sumClose = 0;

		//	//for (int i = 0; i < model.Series["@equity"].list.Count; i++)
		//	int i = 0, iOperate = 0;
		//	foreach (decimal d in model.Series["@equity"].list)
		//	{
		//		while (iOperate < model.Operations.Count)
		//		{
		//			var operate = model.Operations[iOperate];
		//			if (operate.D <= ws.dt[i])
		//			{
		//				if (model.Operations[iOperate].Offset == OffsetFlag.Open)
		//				{
		//					if (feeOpen < 1)
		//						sumOpen += operate.Price * ws.InstrumentField.VolumeMultiple * feeOpen * operate.Lots;
		//					else
		//						sumOpen += feeOpen * operate.Lots;
		//				}
		//				else
		//				{
		//					if (feeOpen < 1)
		//						sumClose += operate.Price * ws.InstrumentField.VolumeMultiple * feeOpen * operate.Lots;
		//					else
		//						sumClose += feeOpen * operate.Lots;
		//				}
		//				iOperate++;
		//			}
		//			else
		//				break;
		//		}

		//		equity.Add(d - sumOpen - sumClose);
		//		i++;
		//	}
		//}

		private void checkBoxMA_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkBoxMA.Checked)
			{
				for (int i = 1; i < this.chartDB1.Series.Count; i++)
					this.chartDB1.Series.RemoveAt(i);
				this.chartDB1.Series.Add("MA");
				this.chartDB1.Series["MA"].ChartType = SeriesChartType.Line;
				this.chartDB1.Series["MA"].ChartArea = this.chartDB1.ChartAreas[0].Name;
				this.chartDB1.Series["MA"].BorderWidth = 2;
				this.chartDB1.Series["MA"].BorderColor = Color.LightGoldenrodYellow;
				this.chartDB1.Series["MA"].IsXValueIndexed = true;
				numericUpDownMALen_ValueChanged(null, null);
			}
			else if (this.chartDB1.Series.Count > 1)
				this.chartDB1.Series.RemoveAt(1);
			this.numericUpDownMALen.Enabled = this.checkBoxMA.Checked;
		}

		private void numericUpDownMALen_ValueChanged(object sender, EventArgs e)
		{
			int len = (int)this.numericUpDownMALen.Value;
			this.chartDB1.DataManipulator.FinancialFormula(FinancialFormula.MovingAverage, len.ToString(), this.chartDB1.Series[0].Name + ":Y", "MA:Y");
			for (int i = 0; i < len - 1; i++)
				this.chartDB1.Series[1].Points.InsertXY(i, this.chartDB1.Series[0].Points[i].XValue, 0);//decimal.NaN);
		}

		private void buttonZoonOut_Click(object sender, EventArgs e)
		{
			ZoomOut(this.chartDB1);
		}

		private void buttonZoomIn_Click(object sender, EventArgs e)
		{
			ZoomIn(this.chartDB1);
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
	}
}
