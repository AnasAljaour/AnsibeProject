document.addEventListener('DOMContentLoaded', (event) => {
    
    let button = document.querySelector('#menu-btn');
    button.addEventListener('click', hideSideBar);
    let toggle = document.getElementById('user-btn');
    toggle.style.display = 'inline-block';
    toggle.addEventListener('click', swapView);
});
function getAnsibeBySelectedYear(selected) {
    let request = {
        Key: "Id",
        Value: selected.value
    }
    $.ajax({
        url: '/Home/getAnsibeBasedOnSelectedYear',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request),
        success: function (response) {
            success(response);
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);

        }
    });
}

function success(response) {

    let tbody = document.getElementById('Ansibe-tbody');
    tbody.innerHTML = '';

    for (let i = 0; i < response.length; i++) {
        row = document.createElement("tr");
        idCell = document.createElement('td');
        idCell.textContent = response[i].id;
        row.append(idCell);
        yearCell = document.createElement("td");
        yearCell.textContent = response[i].year;
        row.append(yearCell);


        let button = document.createElement("Button");
        button.textContent = "Show";
        button.addEventListener("click", function () {
            getTable(button)

        });
        button.classList.toggle("inline-btn");
        let buttonCell = document.createElement('td');
        buttonCell.appendChild(button);
        row.append(buttonCell);


        let editButton = document.createElement("Button");
        editButton.textContent = "Edit";
        editButton.addEventListener("click", function () {

        });
        editButton.classList.toggle("inline-btn");

        let EditbuttonCell = document.createElement('td');
        EditbuttonCell.appendChild(editButton);
        row.append(EditbuttonCell);

        tbody.appendChild(row);
    }
}
function failed(jqXHR, textStatus, errorThrown) {

    if (jqXHR.responseJSON && jqXHR.responseJSON.error) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! \n" + jqXHR.responseJSON.error,

        });
    } else if (jqXHR.status === 400) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! \n" + jqXHR.responseText,

        });
    } else {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! \n" + jqXHR.responseText,

        });
    }
}
let type = "C";
function getTable(button) {
    let row = button.closest('tr');
    let columns = row.getElementsByTagName('td');
    // selected Id from list
    $.ajax({
        url: '/Home/Show',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(columns[0].textContent),
        dataType: 'html',
        success: function (response) {
            let div = document.getElementById("Ansibe-holder");

            div.innerHTML = response;
            type = "P";
            hideSideBar();
            /*$('#Professor-Ansibe').DataTable({
                "paging": true,         // Enable paging
                "searching": true,      // Enable searching/filtering
                "ordering": true,       // Enable sorting
                "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],  // Customize the number of rows per page
                "language": {           // Customize DataTables text
                    "paginate": {
                        "next": "التالي",
                        "previous": "السابق"
                    },
                    "search": "بحث:",
                    "lengthMenu": "إظهار _MENU_ صفوف"
                    // Add more translations as needed
                },
                "rowGroup": {
                    "dataSrc": [0,1,2,3,12] // Index of the column you want to group by (e.g., 'أسم الأستاذ')
                },
                "columnDefs": [
                    { "visible": true, "targets": 0 } // Hide the grouping column if needed
                ]
            });*/
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);

        }
    });

}
function hideSideBar() {
    let sideBar = document.getElementById('side-bar-containr');
    
    sideBar.classList.toggle('active');
}
function swapView() {
    request = {
        Key: document.getElementById('ansibe-id').value,
        Value: type
    }
    console.log(request.Key);
    $.ajax({
        url: '/Home/Swap',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request),
        dataType: 'html',
        success: function (response) {
            let div = document.getElementById("Ansibe-holder");

            div.innerHTML = response;
            type = (type === 'P') ? 'C' : 'P';
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);

        }

    })
}

window.addEventListener('beforeunload', function (e) {
    
        let toggle = document.getElementById('user-btn');
        toggle.style.display = 'none';
    
});