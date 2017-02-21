namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public class DaydreamControllerLog : MonoBehaviour
    {
        public bool consoleLogging;
        public bool csvLogging;
        public string csvPath = @"d:\ControllerData.csv";

        private StreamWriter writer;

        protected virtual void OnEnable()
        {
            if (csvLogging)
            {
                ControllerCSVStart();
            }
        }

        void Update()
        {
            if (csvLogging)
            {
                ControllerCSVWrite();
            }
            if (consoleLogging)
            {
                Debug.Log(GetControllerDataString());
            }
        }

        private void ControllerCSVStart()
        {
            writer = new StreamWriter(csvPath);
            Debug.Log("Logging Controller data to " + csvPath);
            writer.WriteLine(CSVHeader());
        }

        private string CSVHeader()
        {
            string header = "timeTT,orientX,orientY,orientZ,gyroX,gyroY,gyroZ,accelX,accelY,accelZ,appButton,clickButton,touchX,touchY";
            return header;
        }

        private void ControllerCSVWrite()
        {
            writer.WriteLine(GetControllerDataCSV());
        }

        private string GetControllerDataCSV()
        {
            List<string> values = new List<string>();

            values.Add(Time.time.ToString());

            Quaternion orient = GvrController.Orientation;
            Vector3 angles = orient.eulerAngles;
            values.AddRange(new string[] { angles.x.ToString("N3"), angles.y.ToString("N3"), angles.z.ToString("N3") });

            Vector3 gyro = GvrController.Gyro;
            values.AddRange(new string[] { gyro.x.ToString("N3"), gyro.y.ToString("N3"), gyro.z.ToString("N3") });

            Vector3 accel = GvrController.Accel;
            values.AddRange(new string[] { accel.x.ToString("N3"), accel.y.ToString("N3"), accel.z.ToString("N3") });

            if (GvrController.AppButtonDown)
            {
                values.Add("down");
            }
            else if (GvrController.AppButtonUp)
            {
                values.Add("up");
            }
            else if (GvrController.AppButton)
            {
                values.Add("pressed");
            }
            else
            {
                values.Add("");
            }

            if (GvrController.ClickButtonDown)
            {
                values.Add("down");
            }
            else if (GvrController.ClickButtonUp)
            {
                values.Add("up");
            }
            else if (GvrController.ClickButton)
            {
                values.Add("pressed");
            }
            else
            {
                values.Add("");
            }

            if (GvrController.IsTouching)
            {
                Vector2 pos = GvrController.TouchPos;
                values.AddRange(new string[] { pos.x.ToString("N3"), pos.y.ToString("N3") });
            }
            else
            {
                values.AddRange(new string[] { "", "" });
            }

            return string.Join(", ", values.ToArray());
        }


        //====================
        private string GetControllerDataString()
        {
            string log = "";

            Quaternion orient = GvrController.Orientation;
            Vector3 angles = orient.eulerAngles;
            log += "Orient: " + angles.ToString("N3");
            //log += "\n";

            Vector3 gyro = GvrController.Gyro;
            log += " Gyro: " + gyro.ToString("N3");
            //log += "\n";

            Vector3 accel = GvrController.Accel;
            log += " Accel: " + accel.ToString("N3");
            //log += "\n";

            if (GvrController.AppButtonDown)
            {
                log += " App: down";
            }
            else if (GvrController.AppButtonUp)
            {
                log += " App: up";
            }
            else if (GvrController.AppButton)
            {
                log += " App: yes";
            }

            if (GvrController.ClickButtonDown)
            {
                log += " Click: down";
            }
            else if (GvrController.ClickButtonUp)
            {
                log += " Click: up";
            }
            else if (GvrController.ClickButton)
            {
                log += " Click: yes";
            }

            if (GvrController.IsTouching)
            {
                Vector2 pos = GvrController.TouchPos;
                log += " Touch: " + pos.ToString("N3");
            }

            return log;
        }

    }
}