using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KanbanBoardSystem.App
{
    public static class RetryPolicy
    {
        
        public static async Task ExecuteWithRetryAsync(Func<Task> action, int maxRetries = 3)
        {
            int delay = 1000; 
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await action();
                    return; 
                }
                catch (Exception ex)
                {
                    if (i == maxRetries - 1)
                    {
                        
                        MessageBox.Show($"Не вдалося зберегти дані після {maxRetries} спроб. Помилка: {ex.Message}", 
                            "Помилка системи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                    
                    
                    await Task.Delay(delay);
                    delay *= 2; 
                }
            }
        }
    }
}