// Function to generate todo items from JSON data

function generateTodoItems(todoData) {
    const todoListContainer = document.querySelector('.row.mx-1.px-5.pb-3.w-80 .col');
    todoListContainer.innerHTML = '';

    todoData.forEach(todo => {
        const isCompleted = todo.isCompleted;
        const id = todo.id;
        const checkIconClass = isCompleted ? 'fa fa-check-square-o text-primary btn m-0 p-0 ' : 'fa fa-square-o text-primary btn m-0 p-0';
        const uncheckIconClass = isCompleted ? 'fa fa-square-o text-primary btn m-0 p-0 d-none' : 'fa fa-check-square-o text-primary btn m-0 p-0 d-none';


        const formattedDate = formatDate(new Date(todo.dateTime));

        const todoItemDiv = document.createElement('div');
        todoItemDiv.classList.add('row', 'px-3', 'align-items-center', 'todo-item', 'rounded');

        const todoHtml = `
            <div class="col-auto m-1 p-0 d-flex align-items-center">
                <h2 class="m-0 p-0">
                    <i class="${checkIconClass}" data-toggle="tooltip" data-placement="bottom" onclick="toggleIcon(this,${id})" title="Mark as complete"></i>
                    <i class="${uncheckIconClass}" data-toggle="tooltip" data-placement="bottom" onclick="toggleIcon(this,${id})" title="Mark as todo"></i>
                </h2>
            </div>
            <div class="col px-1 m-1 d-flex align-items-center">
                <input type="text" class="form-control form-control-lg border-0 edit-todo-input bg-transparent rounded px-3" readonly value="${todo.todo}" title="${todo.todo}" />
                <input type="text" class="form-control form-control-lg border-0 edit-todo-input rounded px-3 d-none" value="${todo.todo}" />
            </div>
            <div class="col-auto m-1 p-0 px-3 d-none">
                <!-- Optional: Add any due date information here if needed -->
            </div>
            <div class="col-auto m-1 p-0 todo-actions">
                <div class="row d-flex align-items-center justify-content-end">
                    <h5 class="m-0 p-0 px-2">
                        <i class="fa fa-pencil text-info btn m-0 p-0" data-toggle="tooltip" data-placement="bottom" onclick="makeEditable(this,${todo.id})" title="Edit todo" ></i>
                    </h5>
                    <h5 class="m-0 p-0 px-2">
                        <i class="fa fa-trash-o text-danger btn m-0 p-0" data-toggle="tooltip" data-placement="bottom" title="Delete todo" data-id="${todo.id}" onclick="deleteTodoItem(${todo.id})"></i>
                    </h5>
                </div>
                <div class="row todo-created-info">
                    <div class="col-auto d-flex align-items-center pr-2">
                        <i class="fa fa-info-circle my-2 px-2 text-black-50 btn" data-toggle="tooltip" data-placement="bottom" title="Created date"></i>
                        <label class="date-label my-2 text-black-50">${formattedDate}</label>
                    </div>
                </div>
            </div>
        `;

        todoItemDiv.innerHTML = todoHtml;
        todoListContainer.appendChild(todoItemDiv);
    });
}

function makeEditable(editIcon, id) {
    // Find the closest todo item container
    const todoItem = editIcon.closest('.todo-item');

    // Get the input field and make it editable
    const todoInput = todoItem.querySelector('.edit-todo-input');
    todoInput.removeAttribute('readonly');  // Make the input editable
    todoInput.focus(); // Focus on the input field

    // Get the due date label and replace it with a date input field
    const dateLabel = todoItem.querySelector('.date-label');
    const currentDate = dateLabel.textContent;
    dateLabel.innerHTML = `<input type="date" class="form-control form-control-sm" value="${new Date(currentDate).toISOString().split('T')[0]}">`;
    todoInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            // Get the updated text and date values
            const updatedText = todoInput.value.trim();
            const dateInput = dateLabel.querySelector('input[type="date"]');
            const updatedDate = dateInput.value;


            // Call function to update the backend
            updateTodoItem(id, updatedText, updatedDate, todoItem);
        }
    });
}
function updateTodoItem(id, updatedText, updatedDate) {
    // Define the API URL with the specified ID
    const apiUrl = `http://localhost:5260/api/TodoLists/update/${id}`;
    const token = localStorage.getItem('authToken');
    // Prepare the data object
    const data = {
        dateTime: updatedDate,
        todo: updatedText
    };

    fetch(apiUrl, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to update todo item');
            }

            // Check if there's a JSON response
            const contentType = response.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                return response.json(); // If the response is JSON, parse it
            } else {
                return response.text(); // Otherwise, parse as text or handle accordingly
            }
        })
        .then(data => {
            console.log('Todo item updated successfully:', data);
            fetchAndGenerateTodos(); // Refresh the todo list
        })
        .catch(error => {
            console.error('Error updating todo item:', error);
        });
}

function formatDate(date) {
    return (
        date.getDate() +
        "/" +
        (date.getMonth() + 1) +
        "/" +
        date.getFullYear()
    );
}
function toggleIcon(element, id) {
    if (element.className === 'fa fa-check-square-o text-primary btn m-0 p-0') {
        element.className = 'fa fa-square-o text-primary btn m-0 p-0';
        element.setAttribute('title', 'Mark as todo');
        updateTodoStatus(id, false);


    } else if (element.className === 'fa fa-square-o text-primary btn m-0 p-0') {
        element.className = 'fa fa-check-square-o text-primary btn m-0 p-0';
        element.setAttribute('title', 'Mark as complete');
        updateTodoStatus(id, true);

    }
}
function updateTodoStatus(id, isCompleted) {
    const token = localStorage.getItem('authToken');
    fetch(`http://localhost:5260/api/TodoLists/updateStatus/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ isCompleted: isCompleted })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to update todo status');
            }
            return response.json();
        })
        .then(data => {
            console.log('Todo status updated:', data);
        })
        .catch(error => {
            console.error('Error updating todo status:', error);
        });
}

function addTodo() {
    const todoInput = document.querySelector('.add-todo-input');
    const dueDateInput = document.querySelector('.due-date-label').textContent;
    console.log(dueDateInput);
    const todo = todoInput.value.trim();
    const token = localStorage.getItem('authToken');
    if (todo) {
        let dueDate;
        if (dueDateInput && dueDateInput !== 'Due date not set') {
            dueDate = new Date(dueDateInput);
            console.log('Parsed dueDate:', dueDate);
            console.log('Is dueDate valid?', !isNaN(dueDate.getTime()));
            if (isNaN(dueDate.getTime())) {
                console.error('Invalid due date');
                dueDate = null;
            } else {
                dueDate = dueDate.toISOString();
            }
        } else {
            dueDate = null;
        }

        const data = {
            todo: todo,
            dateTime: dueDate
        };

        fetch('http://localhost:5260/api/TodoLists/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(() => {
                todoInput.value = '';
                document.querySelector('.due-date-label').textContent = 'Due date not set';

                // Refresh the todo list
                fetchAndGenerateTodos();
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
    }
    fetchAndGenerateTodos();
}
function fetchAndGenerateTodos() {
    const token = localStorage.getItem('authToken');

    fetch('http://localhost:5260/api/TodoLists', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': `application/json`
        }

    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Fetched data:', data);
            generateTodoItems(data);
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

function deleteTodoItem(id) {
    const token = localStorage.getItem('authToken');
    fetch(`http://localhost:5260/api/TodoLists/Delete/${id}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log(`Todo item with ID ${id} deleted successfully`);
            fetchAndGenerateTodos();
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
    fetchAndGenerateTodos();
}

let currentPage = 1;
const pageSize = 10;
function fetchTodosWithPagination(pageNumber = 1) {
    const token = localStorage.getItem('authToken');

    fetch(`http://localhost:5260/api/TodoLists?pageNumber=${pageNumber}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Fetched data:', data);  // Log the data to inspect its structure
            if (Array.isArray(data.todoLists)) {
                generateTodoItems(data.todoLists);  // Pass the todoLists array to the function
            } else {
                console.error('Error: todoLists is not an array', data.todoLists);
            }
            generatePaginationControls(data.paginationMetadata);  // Create pagination controls
        })
        .catch(error => console.error('Error fetching todos:', error));
}

function generatePaginationControls(pagination) {
    const paginationContainer = document.querySelector('.pagination');
    paginationContainer.innerHTML = '';  // Clear existing pagination

    const prevPage = document.createElement('li');
    prevPage.className = `page-item ${pagination.currentPage === 1 ? 'disabled' : ''}`;
    prevPage.innerHTML = `<a class="page-link" href="#">Previous</a>`;
    prevPage.addEventListener('click', () => {
        if (pagination.currentPage > 1) {
            fetchTodosWithPagination(pagination.currentPage - 1);
        }
    });
    paginationContainer.appendChild(prevPage);

    // Loop to create page numbers dynamically
    for (let i = 1; i <= pagination.totalPages; i++) {
        const pageItem = document.createElement('li');
        pageItem.className = `page-item ${i === pagination.currentPage ? 'active' : ''}`;
        pageItem.innerHTML = `<a class="page-link" href="#">${i}</a>`;
        pageItem.addEventListener('click', () => fetchTodosWithPagination(i));
        paginationContainer.appendChild(pageItem);
    }

    const nextPage = document.createElement('li');
    nextPage.className = `page-item ${pagination.currentPage === pagination.totalPages ? 'disabled' : ''}`;
    nextPage.innerHTML = `<a class="page-link" href="#">Next</a>`;
    nextPage.addEventListener('click', () => {
        if (pagination.currentPage < pagination.totalPages) {
            fetchTodosWithPagination(pagination.currentPage + 1);
        }
    });
    paginationContainer.appendChild(nextPage);
}

// Call this function on page load
window.onload = function () {
    fetchTodosWithPagination(currentPage);
    fetchAndGenerateTodos();

    bootlint.showLintReportForCurrentDocument([], {
        hasProblems: false,
        problemFree: false
    });
    document.querySelector('.btn.btn-primary').addEventListener('click', addTodo);

    $('[data-toggle="tooltip"]').tooltip();

    function formatDate(date) {
        return (
            date.getDate().toString().padStart(2, '0') +
            "/" +
            (date.getMonth() + 1).toString().padStart(2, '0') +
            "/" +
            date.getFullYear()
        );
    }

    var currentDate = formatDate(new Date());

    $(".due-date-button").datepicker({
        format: "dd/mm/yyyy",
        autoclose: true,
        todayHighlight: true,
        startDate: currentDate,
        orientation: "bottom right"
    });

    $(".due-date-button").on("click", function () {
        $(this).datepicker("show");
    });

    $(".due-date-button").on("changeDate", function (dateChangeEvent) {
        const selectedDate = formatDate(dateChangeEvent.date);
        $(".due-date-label").text(selectedDate).removeClass('d-none');
        $(".clear-due-date-button").removeClass('d-none');
    });

    $(".clear-due-date-button").on("click", function () {
        $(".due-date-label").text("Due date not set").addClass('d-none');
        $(".clear-due-date-button").addClass('d-none');
    });
};

document.addEventListener('DOMContentLoaded', () => {
    const logoutButton = document.getElementById('logout-button');

    if (logoutButton) {
        logoutButton.addEventListener('click', () => {
            const token = localStorage.getItem('authToken');

            if (token) {
                // Send token to backend for logout
                fetch('http://localhost:5260/User/logout', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}` // Correctly using the token
                    },
                    body: JSON.stringify(token) // Correctly sending token as object in body
                })
                    .then(response => response.json())
                    .then(data => {
                        console.log(data.message || "Logged out successfully.");
                        // Clear token and session data
                        localStorage.removeItem('authToken');
                        sessionStorage.clear();
                        deleteAllCookies();

                        // Redirect after logout
                        window.location.href = 'http://localhost:8000';
                    })
                    .catch(error => console.error('Error:', error));
            } else {
                console.error('No token found in local storage.');
            }
        });
    }
});

// Function to delete all cookies
function deleteAllCookies() {
    const cookies = document.cookie.split(';');
    cookies.forEach(cookie => {
        const [name] = cookie.split('=');
        document.cookie = `${name.trim()}=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT;`;
        document.cookie = `${name.trim()}=; path=/; domain=${window.location.hostname}; expires=Thu, 01 Jan 1970 00:00:00 GMT;`;
    });
}
