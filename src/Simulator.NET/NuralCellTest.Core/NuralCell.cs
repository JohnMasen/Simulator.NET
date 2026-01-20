using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace NuralCellTest.Core
{
    public class NuralCell
    {
        public NuraCellStatusEnum CellStatus { get; set; }

        public delegate float StepDeltaDelegate(int frame);

        private int raisingFrames = 0;
        private int fallingFrames = 0;
        private int highVoltageFrames = 0;
        private int overDischargeFrames = 0;
        private int restoringFrames = 0;
        public float InputVoltage { get; set; }
        public bool IsRaising { get; private set; }
        public float ActivationVoltage { get; set; }
        /// <summary>
        /// 神经细胞电压
        /// </summary>
        public float Voltage { get; set; }
        public float FallingOverVoltage { get; set; } = 0.1f;

        public StepDeltaDelegate RaiseStepper { get; set; } = _ => 0.1f;
        public StepDeltaDelegate FallStepper { get; set; } = _ => -0.1f;
        public StepDeltaDelegate RestoreStepper { get; set; } = _ => 0.1f;

        public float CellBaseVoltage => BaseVoltage + InputVoltage;

        /// <summary>
        /// 神经细胞基础电压，非激活状态下细胞电压将会趋向于该电压
        /// </summary>
        public float BaseVoltage { get; set; }

        /// <summary>
        /// 激发阈值,当电压超过该值持续<see cref="ActivationDelay"/>帧后，触发激活
        /// </summary>
        public float Threashold { get; set; }
        /// <summary>
        /// 高电位激发阈值
        /// </summary>
        public float? ThreasholdHigh { get; set; }

        /// <summary>
        /// 激发延迟,电位必须持续超过阈值若干周期才会触发激活
        /// </summary>
        public int ActivationDelay { get; set; }

        public void Step()
        {
            switch (CellStatus)
            {
                case NuraCellStatusEnum.Idle:
                    Voltage = BaseVoltage + InputVoltage;
                    if (Voltage>=ThreasholdHigh)
                    {
                        changeToRaising();
                        break;
                    }
                    if (Voltage>=Threashold)
                    {
                        
                        if (highVoltageFrames>=ActivationDelay)
                        {
                            changeToRaising();
                        }
                        else
                        {
                            highVoltageFrames++;
                        }
                    }
                    else
                    {
                        highVoltageFrames = 0;
                    }
                        break;
                case NuraCellStatusEnum.Raising:
                    var dvr = RaiseStepper(raisingFrames++);
                    if (Voltage+dvr>=ActivationVoltage)
                    {
                        Voltage = ActivationVoltage;
                        changeToFalling();
                    }
                    else
                    {
                        Voltage += dvr;
                    }
                        
                    break;
                case NuraCellStatusEnum.Falling:
                    Voltage += FallStepper(fallingFrames);
                    if (Voltage<=BaseVoltage)
                    {
                        overDischargeFrames++;
                        if (Voltage<=BaseVoltage-FallingOverVoltage)
                        {
                            Voltage = BaseVoltage - FallingOverVoltage;
                            changeToRestore();
                        }
                    }
                    break;
                case NuraCellStatusEnum.Restoring:
                    //do restore
                    calculateVoltageDelta(RestoreStepper(restoringFrames++), CellBaseVoltage, changeToIdle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CellStatus));
            }
        }

        private void changeToRaising()
        {
            Debug.WriteLine("Raising start");
            resetFrames();
            CellStatus = NuraCellStatusEnum.Raising;
        }
        private void changeToFalling()
        {
            Debug.WriteLine("Falling start");
            resetFrames();
            CellStatus = NuraCellStatusEnum.Falling;
        }
        private void changeToRestore()
        {
            Debug.WriteLine("Restore start");
            resetFrames();
            CellStatus = NuraCellStatusEnum.Restoring;
        }
        private void changeToIdle()
        {
            Debug.WriteLine("Idle start");
            resetFrames();
            CellStatus = NuraCellStatusEnum.Idle;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void resetFrames()
        {
            raisingFrames = 0;
            fallingFrames = 0;
            overDischargeFrames = 0;
            highVoltageFrames = 0;
            restoringFrames = 0;
        }

        private void calculateVoltageDelta(float delta,float threshHold,Action overflowCallback)
        {
            if (delta>0)
            {
                if (Voltage + delta >= threshHold)
                {
                    Voltage = threshHold;
                    overflowCallback();
                    return;
                }
            }
            else
            {
                if (Voltage + delta<=threshHold)
                {
                    Voltage = threshHold;
                    overflowCallback();
                    return;
                }
            }
            Voltage += delta;
        }


    }
}
