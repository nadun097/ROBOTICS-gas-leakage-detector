using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GasDetection
{
    public partial class Form1 : Form
    {
        SerialPort serialPort1;
        int gasValue;
        int flameValue;
        DateTime startTime;
        Chart gasChart;

        // Variables for tracking red line duration
        DateTime? redLineStartTime = null;
        int redLineElapsedSeconds = 0;
        bool timePassed = false;

        bool normalCommandSent = false;

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            InitializeChart();
            startTime = DateTime.Now;
        }

        private void InitializeSerialPort()
        {
            serialPort1 = new SerialPort("COM3", 9600);
            serialPort1.DataReceived += SerialPort_DataReceived;
            serialPort1.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort1.ReadLine().Trim();


                string[] parts = data.Split(',');
                string gasPart = parts[0];
                string flamePart = parts[1];

                gasValue = int.Parse(gasPart);
                flameValue = int.Parse(flamePart);

                Console.WriteLine(" Gas = " + gasValue + " Flame = " + flameValue);


                UpdateSensorValue(gasValue);
                PlotSensorValue(gasValue);
                UpdateProgressBar(gasValue);


                if (gasValue >= 350)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => btnToggleValve.Text = "Open Valve"));
                        Invoke(new Action(() => lblGasStatus.Text = "Closed"));
                        Invoke(new Action(() => lblWindows.Text = "Open"));
                        Invoke(new Action(() => btnToggleWindow.Text = "Close Window"));
                    }

                    else
                    {
                        btnToggleValve.Text = "Open Valve";
                        lblGasStatus.Text = "Closed";
                        lblWindows.Text = "Open";
                        btnToggleWindow.Text = "Close Window";
                    }

                }
                else
                {
                    if (!btnToggle.Checked)
                    {
                        Invoke(new Action(() => lblWindows.Text = "Closed"));
                        Invoke(new Action(() => btnToggleWindow.Text = "Open Window"));
                    }

                }

                if (flameValue <= 500)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => btnToggleValve.Text = "Open Valve"));
                        Invoke(new Action(() => lblGasStatus.Text = "Closed"));
                        Invoke(new Action(() => lblWindows.Text = "Open"));
                        Invoke(new Action(() => btnToggleWindow.Text = "Close Window"));
                        Invoke(new Action(() => lblFire.Text = "Fire !!!"));
                    }

                    else
                    {
                        btnToggleValve.Text = "Open Valve";
                        lblGasStatus.Text = "Closed";
                        lblWindows.Text = "Open";
                        btnToggleWindow.Text = "Close Window";
                        lblFire.Text = "Fire !!!";
                    }

                }
                else
                {
                    Invoke(new Action(() => lblFire.Text = "Normal"));
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
            }
        }

        private void InitializeChart()
        {
            gasChart = new Chart();
            ChartArea chartArea = new ChartArea("GasSensorChart");

            chartArea.AxisX.Title = "Time";
            chartArea.AxisX.Minimum = DateTime.Now.ToOADate();
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.LabelStyle.Format = "HH:mm";

            chartArea.AxisY.Title = "Gas Value";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = 1000;

            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;

            gasChart.ChartAreas.Add(chartArea);

            Series series = new Series("Gas Value")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.DateTime,
                BorderWidth = 1,
                Color = System.Drawing.Color.Blue
            };

            gasChart.Series.Add(series);
            gasChart.Dock = DockStyle.Top;
            gasChart.Height = 450;
            Controls.Add(gasChart);
        }

        private void PlotSensorValue(int value)
        {
            if (InvokeRequired)
                Invoke(new Action(() => AddChartData(value)));
            else
                AddChartData(value);
        }

        private void AddChartData(int value)
        {
            Series series = gasChart.Series["Gas Value"];
            DateTime currentTime = DateTime.Now;

            series.Points.AddXY(currentTime.ToOADate(), value);

            DateTime windowStartTime = currentTime.AddMinutes(-2);
            gasChart.ChartAreas[0].AxisX.Minimum = windowStartTime.ToOADate();
            gasChart.ChartAreas[0].AxisX.Maximum = currentTime.ToOADate();

            if (series.Points.Count > 1000)
                series.Points.RemoveAt(0);

            if (value > 350)
            {
                series.Color = System.Drawing.Color.Red;

                if (redLineStartTime == null)
                {
                    redLineStartTime = currentTime;
                }
                else
                {
                    redLineElapsedSeconds = (int)(currentTime - redLineStartTime.Value).TotalSeconds;

                    if (redLineElapsedSeconds >= 20 && !timePassed)
                    {
                        SendCommand("timePassed");
                        timePassed = true;
                    }
                }
            }
            else
            {
                if (!normalCommandSent)
                {
                    SendCommand("normal");
                    normalCommandSent = true;
                }

                series.Color = System.Drawing.Color.Blue;

                if (redLineStartTime != null)
                {
                    redLineStartTime = null;
                    redLineElapsedSeconds = 0;
                    timePassed = false;
                    normalCommandSent = false; // Reset the normal command flag
                }
            }
        }

        private void UpdateSensorValue(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => lblGasValue.Text = $"Gas Value: {value}"));
            }
            else
            {
                lblGasValue.Text = $"Gas Value: {value}";
            }

        }

        private void SendCommand(string command)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine(command);
            }
            else
            {
                MessageBox.Show("Serial port is not open.");
            }
        }

        private void btnToggleValve_Click(object sender, EventArgs e)
        {
            if (btnToggleValve.Text == "Open Valve")
            {
                Invoke(new Action(() => lblGasStatus.Text = "Opened"));
                SendCommand("OPEN_VALVE");
                btnToggleValve.Text = "Close Valve";
            }
            else
            {
                Invoke(new Action(() => lblGasStatus.Text = "Closed"));
                SendCommand("CLOSE_VALVE");
                btnToggleValve.Text = "Open Valve";
            }
        }

        private void btnToggleWindow_Click(object sender, EventArgs e)
        {
            if (btnToggleWindow.Text == "Open Window")
            {
                SendCommand("OPEN_WINDOW");
                btnToggleWindow.Text = "Close Window";
                lblWindows.Text = "Open";
            }
            else
            {
                SendCommand("CLOSE_WINDOW");
                btnToggleWindow.Text = "Open Window";
                lblWindows.Text = "Closed";
            }
        }

        private void btnToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (btnToggle.Checked)
            {
                lblMode.Text = "Manual";
                SendCommand("ManualWindow");
                btnToggleWindow.Enabled = true;
            }
            else
            {
                lblMode.Text = "Auto";
                SendCommand("AutoWindow");
                btnToggleWindow.Enabled = false;
                if (gasValue < 350)
                {
                    Invoke(new Action(() => btnToggleWindow.Text = "Open Window"));
                    Invoke(new Action(() => lblWindows.Text = "Closed"));
                }
            }
        }

        private void UpdateProgressBar(int value)
        {
            // Ensure that the value is within the progress bar's valid range
            if (progressBar1.InvokeRequired)
            {
                Invoke(new Action(() => progressBar1.Value = value));
                if (value >= 0 && value <= 199)
                {
                    Invoke(new Action(() => progressBar1.Style = MetroFramework.MetroColorStyle.Green));
                    Invoke(new Action(() => lblRisk.Text = "LOW"));
                }
                else if (value >= 200 && value <= 299)
                {
                    Invoke(new Action(() => progressBar1.Style = MetroFramework.MetroColorStyle.Yellow));
                    Invoke(new Action(() => lblRisk.Text = "MEDIUM"));
                }
                else
                {
                    Invoke(new Action(() => progressBar1.Style = MetroFramework.MetroColorStyle.Red));
                    Invoke(new Action(() => lblRisk.Text = "HIGH"));
                }
            }
            else
            {
                progressBar1.Value = value;

                if (value >= 0 && value <= 199)
                {
                    progressBar1.Style = MetroFramework.MetroColorStyle.Green;
                    lblRisk.Text = "LOW";
                }
                else if (value >= 200 && value <= 299)
                {
                    progressBar1.Style = MetroFramework.MetroColorStyle.Yellow;
                    lblRisk.Text = "MEDIUM";
                }
                else
                {
                    progressBar1.Style = MetroFramework.MetroColorStyle.Red;
                    lblRisk.Text = "HIGH";
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendCommand("STOP_ALARM");
        }
    }
}