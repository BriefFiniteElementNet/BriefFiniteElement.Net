## Known Issues

- `ConcentratedLoad` is not usable yet (due to bugs)
- `PartialNonUniformLoad` is not usable yet (due to bugs)
- `TetraHedron` not implemented fully yet
- Triangle shell internal force Buggy (rest works fine)
- `StiffnessCenterFinder` not implemented
- `BarElement.EndRelease` stuff not implemented with bar element -  Solved 6/17/2019
- `BarElement.GetInternalForce()` and `BarElement.GetExactInternalForce()` can have bugs under some situations.
- `TriangleElement` and `QuadrilaturalElement` are not tested yet
- `Model.Solve_MPC()` still need work for displacemenet permutation stuff.



## To do

unit test for this objectives must be implemented:

- LoadInternalForce_uniformload_eulerbernoullybeam_dirY
- LoadInternalForce_uniformload_eulerbernoullybeam_dirY
- LoadInternalForce_uniformload_truss
- LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirY_fz
- LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirY_my
- LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirZ_mz
- LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirZ_fy
- LoadInternalForce_concentratedLLoad_truss_fx
- LoadInternalForce_concentratedLLoad_shaft_mx
- Add validation for `TriangleElement` and `QuadrilaturalElement` for both membrane and plate-bending behavior.
- Update getting started section on read the doc documentation as project migrated to sdk-style projects and vs2017 is least version for building the library
- Also include working with nuget package and get project from nuget instead of build in getting started section.
- Update documentation for `PartialNonUniformLoad` in documentation and add examples