using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilsLib.Utils
{
    public class ChangePointBase
    {
        // This example creates a time series (list of Data with the i-th element
        // corresponding to the i-th time slot). The estimator is applied then to
        // identify points where data distribution changed.
        public static float GetLastChangePoint(List<TimeSeriesData> list)
        {
            // Create a new ML context, for ML.NET operations. It can be used for
            // exception tracking and logging, as well as the source of randomness.
            var ml = new MLContext();

            // Convert data to IDataView.
            var dataView = ml.Data.LoadFromEnumerable(list);

            // Setup estimator arguments
            string outputColumnName = nameof(ChangePointPrediction.Prediction);
            string inputColumnName = nameof(TimeSeriesData.Value);

            // The transformed data.
            var transformedData = ml.Transforms.DetectIidChangePoint(
                outputColumnName, inputColumnName, 95.0d, list.Count / 4).Fit(dataView)
                .Transform(dataView);

            // Getting the data of the newly created column as an IEnumerable of
            // ChangePointPrediction.
            var predictionColumn = ml.Data.CreateEnumerable<ChangePointPrediction>(
                transformedData, reuseRowObject: false);

            var prediction = predictionColumn.Last();
            return (float)prediction.Prediction[2];
        }

        private static void PrintPrediction(float value, ChangePointPrediction prediction)
        {
            Console.WriteLine("Data\t\tAlert\t\tScore\t\tP-Value\t\tMartingale value");
            Console.WriteLine("{0}\t\t{1}\t\t{2:0.00}\t\t{3:0.00}\t\t{4:0.00}", value,
            prediction.Prediction[0], prediction.Prediction[1],
            prediction.Prediction[2], prediction.Prediction[3]);
        }

        class ChangePointPrediction
        {
            [VectorType(4)]
            public double[] Prediction { get; set; }
        }

        public class TimeSeriesData
        {
            public float Value;

            public TimeSeriesData(float value)
            {
                Value = value;
            }
        }
    }
}
