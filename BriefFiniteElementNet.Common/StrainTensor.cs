using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 3x3 tensor for stress in 3D
    /// </summary>
    /// <remarks>
    /// this is the order: 
    /// | ε₁₁ γ₁₂ γ₁₃| = |StrainTensor3D.S11 StrainTensor3D.S12 StrainTensor3D.S13|
    /// | γ₂₁ ε₂₂ γ₂₃| = |StrainTensor3D.S21 StrainTensor3D.S22 StrainTensor3D.S23|
    /// | γ₃₁ γ₃₂ ε₃₃| = |StrainTensor3D.S31 StrainTensor3D.S32 StrainTensor3D.S33|
    /// </remarks>
    public struct StrainTensor
    {
        private double s33;
        private double s11;
        private double s12;
        private double s13;
        private double s21;
        private double s22;
        private double s23;
        private double s31;
        private double s32;

        public double S11
        {
            get { return s11; }
            set
            {
                s11 = value;
            }
        }
        public double S12
        {
            get { return s12; }
            set
            {
                s12 = value;
            }
        }
        public double S13
        {
            get { return s13; }
            set
            {
                s13 = value;
            }
        }
        public double S21
        {
            get { return s21; }
            set
            {
                s21 = value;
            }
        }
        public double S22
        {
            get { return s22; }
            set
            {
                s22 = value;
            }
        }
        public double S23
        {
            get { return s23; }
            set
            {
                s23 = value;
            }
        }
        public double S31
        {
            get { return s31; }
            set
            {
                s31 = value;
            }
        }
        public double S32
        {
            get { return s32; }
            set
            {
                s32 = value;
            }
        }
        public double S33
        {
            get { return s33; }
            set
            {
                s33 = value;
            }
        }
    }
}
