// using Model.Domain;
//
// namespace Model.NewsProcessing;
//
// public class RawNewsProcessor
// {
//     private readonly NewsProcessor _newsProcessor = new NewsProcessor();
//     
//     public async Task Process(RawNewsMessage rawNews)
//     {
//         var newsProcessorRequest = new NewsProcessorRequest(newsUrl: "https://www.rbc.ru/finances/06/06/2025/68403e4c9a7947dda98935cf");
//         var processResult = await _newsProcessor.ProcessAsync(newsProcessorRequest);
//         
//         switch (processResult)
//         {
//             case SuccessResult successResult:
//                 await Task.WhenAll(
//                     PublishHypothesesAsync(successResult),
//                     PublishRawNewsSuccessProcessingSignalAsync(newsProcessorRequest, successResult));
//                 break;
//             case ErrorResult failedResult:
//                 await PublishRawNewsFailedProcessingSignalAsync(newsProcessorRequest, failedResult);
//                 break;
//         }
//     }
//
//     private async Task PublishHypothesesAsync(INewsProcessorResult newsProcessingResult)
//     {
//     }
//
//     private async Task PublishRawNewsSuccessProcessingSignalAsync(NewsProcessorRequest request, INewsProcessorResult newsProcessingResult)
//     {
//     }
//
//     private async Task PublishRawNewsFailedProcessingSignalAsync(NewsProcessorRequest request, INewsProcessorResult newsProcessingResult)
//     {
//     }
// }