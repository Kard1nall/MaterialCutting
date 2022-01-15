using BackToTheFuture.Models;
using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackToTheFuture.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(InputData inputData)
        {

            SampleParam[] sampleParams = new SampleParam[]
            {
            new SampleParam {CountForOne = 6, SquareForOne = 3.4 },
            new SampleParam {CountForOne = 2, SquareForOne = 5.4},
            new SampleParam {CountForOne = 7, SquareForOne = 3.0 }
            };

            SampleType[] sampleTypes = new SampleType[]
            {
              new SampleType { Id = 1, Count1 = 1, Count2 = 2, Count3 = 0 },
              new SampleType { Id = 2, Count1 = 1, Count2 = 0, Count3 = 1 },
              new SampleType { Id = 3, Count1 = 0, Count2 = 1, Count3 = 1 },
              new SampleType { Id = 4, Count1 = 0, Count2 = 0, Count3 = 3 },

            };

            return View("Index", new InputData() { sampleParams = sampleParams, sampleTypes = sampleTypes, areaSquare = 16, totalCount = 50 });
        }

        [HttpGet]
        public ActionResult Calculate()
        {
            return View(new OutputData());
        }

        [HttpPost]
        public ActionResult Calculate(InputData inputData)
        {

            List<SampleType> solverList = new List<SampleType>();

            solverList.AddRange(inputData.sampleTypes);

            SolverContext solverContext = SolverContext.GetContext();
            Model model = solverContext.CreateModel();

            Set users = new Set(Domain.Any, "users");


            Parameter Count1 = new Parameter(Domain.IntegerNonnegative, "Count1", users);
            Count1.SetBinding(solverList, "Count1", "Id");
            Parameter Count2 = new Parameter(Domain.IntegerNonnegative, "Count2", users);
            Count2.SetBinding(solverList, "Count2", "Id");
            Parameter Count3 = new Parameter(Domain.IntegerNonnegative, "Count3", users);
            Count3.SetBinding(solverList, "Count3", "Id");


            int border1 = inputData.totalCount * inputData.sampleParams[0].CountForOne;
            int border2 = inputData.totalCount * inputData.sampleParams[1].CountForOne;
            int border3 = inputData.totalCount * inputData.sampleParams[2].CountForOne;

            model.AddParameters(Count1, Count2, Count3);
            Decision choose = new Decision(Domain.IntegerNonnegative, "choose", users);
            model.AddDecision(choose);
            model.AddGoal("goal", GoalKind.Minimize, Model.Sum(Model.ForEach(users, Id => choose[Id] * (inputData.areaSquare - 
            (inputData.sampleParams[0].SquareForOne*Count1[Id]+ inputData.sampleParams[1].SquareForOne * Count2[Id]+ inputData.sampleParams[2].SquareForOne * Count3[Id])))));

           
            model.AddConstraint("Rav1", Model.Sum(Model.ForEach(users, Id => Count1[Id] * choose[Id])) == border1);
            model.AddConstraint("Rav2", Model.Sum(Model.ForEach(users, Id => Count2[Id] * choose[Id])) == border2);
            model.AddConstraint("Rav3", Model.Sum(Model.ForEach(users, Id => Count3[Id] * choose[Id])) >= border3);


            OutputData outputData = new OutputData();
            try
            {
                Solution solution = solverContext.Solve();

               

                for (int i = 0; i < solverList.Count; i++)
                {
                    outputData.chooses[i] = (int)choose.GetDouble(solverList[i].Id);
                  
                }

                for (int i = 0; i < solverList.Count; i++)
                {
                    outputData.border1 += solverList[i].Count1 * outputData.chooses[i];
                    outputData.border2 += solverList[i].Count2 * outputData.chooses[i];
                    outputData.border3 += solverList[i].Count3 * outputData.chooses[i];


                }

                outputData.remainder1 = Math.Round(Convert.ToDouble(outputData.chooses[0] * (inputData.areaSquare -
(inputData.sampleParams[0].SquareForOne * inputData.sampleTypes[0].Count1 + inputData.sampleParams[1].SquareForOne * inputData.sampleTypes[0].Count2 + inputData.sampleParams[2].SquareForOne * inputData.sampleTypes[0].Count3))),0);
                outputData.remainder2 = Math.Round(Convert.ToDouble(outputData.chooses[1] * (inputData.areaSquare -
(inputData.sampleParams[0].SquareForOne * inputData.sampleTypes[1].Count1 + inputData.sampleParams[1].SquareForOne * inputData.sampleTypes[1].Count2 + inputData.sampleParams[2].SquareForOne * inputData.sampleTypes[1].Count3))), 0);
                outputData.remainder3 = Math.Round(Convert.ToDouble(outputData.chooses[2] * (inputData.areaSquare -
                (inputData.sampleParams[0].SquareForOne * inputData.sampleTypes[2].Count1 + inputData.sampleParams[1].SquareForOne * inputData.sampleTypes[2].Count2 + inputData.sampleParams[2].SquareForOne * inputData.sampleTypes[2].Count3))), 0);
                outputData.remainder4 = Math.Round(Convert.ToDouble(outputData.chooses[3] * (inputData.areaSquare -
(inputData.sampleParams[0].SquareForOne * inputData.sampleTypes[3].Count1 + inputData.sampleParams[1].SquareForOne * inputData.sampleTypes[3].Count2 + inputData.sampleParams[2].SquareForOne * inputData.sampleTypes[3].Count3))), 0);

                return View(outputData);

            }
            catch (Exception ex)
            {

                return View("Index", inputData);
            }


        }
    }
}