Imports BriefFiniteElementNet
Imports BriefFiniteElementNet.Elements

Module Module1
    Sub Main()
        Dim Model As New Model()

        Dim n1 As New Node(0, 0, 0)
        Dim n2 As New Node(1, 0, 0)

        n1.Constraints = Constraints.Fixed
        n2.Constraints = Constraints.Released

        Model.Nodes.Add(n1)
        Model.Nodes.Add(n2)

        Dim elm = New BarElement(n1, n2)

        Model.Elements.Add(elm)

        'Section's second area moments Iy and Iz = 8.3*10^-6, area = 0.01
        elm.Section = New Sections.UniformParametric1DSection(0.01, 0.0000083, 0.0000083, 0.0000166)

        'Elastic mudule (E) is 210E9 (210 * 10^9) And poisson ratio = 0.3
        elm.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210000000000.0, 0.3)

        Dim Load As New NodalLoad()
        Dim frc As New Force()

        frc.Fz = 1000 '1kN force In Z direction
        Load.Force = frc

        n2.Loads.Add(Load)

        ' Model.Solve_MPC() 'Or model.Solve()
        model.Solve()

        Dim d2 = n2.GetNodalDisplacement()

        Dim v11 = Math.Round(d2.DZ, 6).ToString
        Dim v12 = Math.Round((d2.DZ * 1000), 6).ToString
        Dim v21 = Math.Round(d2.RY, 6).ToString
        Dim v22 = Math.Round((d2.RY * 180.0 / Math.PI), 6).ToString

        Dim OutputString As New Text.StringBuilder
        With OutputString
            .AppendLine($"The Nodal displacement in the z-axis is: {v11} m (Or: {v12} mm)")
            .AppendLine($"The Nodal rotation along the y-axis is: {v11} rad (Or: {v22} deg)")
        End With

        Console.WriteLine(OutputString.ToString)
    End Sub
End Module
