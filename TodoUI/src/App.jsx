import React, { useState, useEffect } from 'react';
import './App.css';

const API_BASE_URL = 'https://localhost:7298/api/tasks';

function App() {
  const [tasks, setTasks] = useState([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(API_BASE_URL);
      if (!response.ok) {
        throw new Error('Failed to fetch tasks');
      }
      const data = await response.json();
      setTasks(data);
    } catch (err) {
      setError(err.message || 'Failed to load tasks');
      console.error('Error fetching tasks:', err);
    } finally {
      setLoading(false);
    }
  };

  const addTask = async () => {
    const trimmedInput = input.trim();
    if (!trimmedInput) {
      setError('Task title cannot be empty');
      return;
    }

    setError('');
    try {
      const response = await fetch(API_BASE_URL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ title: trimmedInput }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData || 'Failed to add task');
      }

      setInput('');
      await fetchTasks();
    } catch (err) {
      setError(err.message || 'Failed to add task');
      console.error('Error adding task:', err);
    }
  };

  const toggleTask = async (id, currentStatus) => {
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL}/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ isCompleted: !currentStatus }),
      });

      if (!response.ok) {
        throw new Error('Failed to update task');
      }

      await fetchTasks();
    } catch (err) {
      setError(err.message || 'Failed to update task');
      console.error('Error updating task:', err);
    }
  };

  const deleteTask = async (id) => {
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL}/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error('Failed to delete task');
      }

      await fetchTasks();
    } catch (err) {
      setError(err.message || 'Failed to delete task');
      console.error('Error deleting task:', err);
    }
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter') {
      addTask();
    }
  };

  return (
    <div className="app-container">
      <div className="app-card">
        <h1>My Todo List</h1>
        
        {error && <div className="error-message">{error}</div>}
        
        <div className="input-group">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Add a new task..."
            className="task-input"
            disabled={loading}
          />
          <button 
            onClick={addTask} 
            className="add-button"
            disabled={loading}
          >
            {loading ? 'Adding...' : 'Add'}
          </button>
        </div>

        {loading && <p className="loading">Loading...</p>}

        <ul className="task-list">
          {tasks.map((task) => (
            <li key={task.id} className="task-item">
              <div className="task-checkbox-group">
                <input
                  type="checkbox"
                  checked={task.isCompleted}
                  onChange={() => toggleTask(task.id, task.isCompleted)}
                  className="task-checkbox"
                  aria-label={`Mark "${task.title}" as ${task.isCompleted ? 'incomplete' : 'complete'}`}
                />
              </div>
              <div className="task-content">
                <span className={task.isCompleted ? 'task-title completed' : 'task-title'}>
                  {task.title}
                </span>
                <span className="task-date">
                  {new Date(task.createdAt).toLocaleDateString()}
                </span>
              </div>
              <button
                onClick={() => deleteTask(task.id)}
                className="delete-button"
                disabled={loading}
                aria-label={`Delete task "${task.title}"`}
              >
                Delete
              </button>
            </li>
          ))}
        </ul>

        {tasks.length === 0 && !loading && (
          <p className="empty-message">No tasks yet. Add one to get started!</p>
        )}
      </div>
    </div>
  );
}

export default App;
