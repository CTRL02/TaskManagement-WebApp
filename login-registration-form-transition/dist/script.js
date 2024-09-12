document.querySelector('.img__btn').addEventListener('click', function() {
  document.querySelector('.cont').classList.toggle('s--signup');
});

// Register
document.getElementById('registerBtn').addEventListener('click', async () => {
    const userName = document.getElementById('nameInput').value;
    const email = document.getElementById('emailInput').value;
    const password = document.getElementById('passwordInput').value;

    if (!userName || !email || !password) {
        alert('Please fill in all fields.');
        return;
    }

    try {
        const registerData = {
            userName: userName,
            email: email,
            password: password
        };

        const response = await fetch('http://localhost:5260/User/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(registerData)
        });

        const token = await response.text();  

        if (response.ok && token) {

            localStorage.setItem('authToken', token);

            window.location.href = "http://localhost:8000/Todo/Todo.html";
        } else {
            alert('Registration failed.');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred during registration.');
    }
});

document.getElementById('loginBtn').addEventListener('click', async () => {
    const email = document.getElementById('emailLogin').value;
    const password = document.getElementById('passwordLogin').value;

    if (!email || !password) {
        alert('Please fill in both email and password.');
        return;
    }

    try {
        const loginData = {
            email: email,
            password: password
        };

        const response = await fetch('http://localhost:5260/User/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        });

        const token = await response.text();
        console.log(token);

        if (response.ok && token) {
            // Store the token in localStorage
            localStorage.setItem('authToken', token);

            // Redirect to the TodoList page
            window.location.href = "http://localhost:8000/Todo/Todo.html";
        } else {
            alert('Login failed. Please check your credentials.');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred during login.');
    }
});

document.getElementById('googleSignin').addEventListener('click', () => {
    // Open the popup for Google Sign-In
    const popup = window.open('http://localhost:5260/User/google-login', 'google-signin', 'width=600,height=400');

    // Listen for the token sent via postMessage
    window.addEventListener('message', (event) => {
        console.log('Message received from origin:', event.origin); // Log the origin

        if (event.origin !== 'http://localhost:5260') return; // Ensure the message is from the expected origin

        const { token } = event.data;
        if (token) {
            // Save the token in localStorage
            localStorage.setItem('authToken', token);

            // Redirect to /home
            window.location.href = 'http://localhost:8000/Todo/Todo.html';
        } else {
            console.error('No token received.');
        }
    }, false);
});

document.getElementById('googleSignin2').addEventListener('click', () => {
    // Open the popup for Google Sign-In
    const popup = window.open('http://localhost:5260/User/google-login', 'google-signin', 'width=600,height=400');

    // Listen for the token sent via postMessage
    window.addEventListener('message', (event) => {
        console.log('Message received from origin:', event.origin); // Log the origin

        if (event.origin !== 'http://localhost:5260') return; // Ensure the message is from the expected origin

        const { token } = event.data;
        if (token) {
            // Save the token in localStorage
            localStorage.setItem('authToken', token);

            // Redirect to /home
            window.location.href = 'http://localhost:8000/Todo/Todo.html';
        } else {
            console.error('No token received.');
        }
    }, false);
});