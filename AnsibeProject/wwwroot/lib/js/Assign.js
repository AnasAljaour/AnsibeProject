function getAllocationBasedOnYear(selected) {
    let year = selected.value;

    $.ajax({
        url: '/AssignP/getAnsibeByYear',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ year: year }),
        success: function (response) {
            success(response);
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);

        }
    });

}

function success(response) {
    console.log(response);
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
        button.textContent = "show";
        button.addEventListener("click", function () {
            showAllocation(button);
        });
        button.classList.toggle("inline-btn");
        buttonCell = document.createElement('td');
        buttonCell.appendChild(button);
        row.append(buttonCell);


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
let sections;
let type;
function showAllocation(button) {
    let row = button.closest('tr');
    let columns = row.getElementsByTagName('td');
    data = {
        Key: "Id",
        Value: columns[0].textContent
    }
    console.log(columns[0].textContent);



    $.ajax({
        url: '/AssignP/getSectionsOfById',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        dataType: 'json',
        success: function (response) {
            console.log(response);
            sections = response;
            type = "CP";
            $.ajax({
                url: '/AssignP/getSectionOfTheAnsibeById',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                dataType: 'html',
                success: function (response) {

                    document.getElementById('holder').innerHTML = response;
                },
                error: function (xhr, status, error) {
                    failed(xhr, status, error);
                    console.log(error);
                    console.log(xhr.responseText);
                }
            });

        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);
            

        }
    });

}



function swapContent() {
    //if (type === null) return;
    console.log(sections);
    type = (type === "CP") ? "PS" : "CP";
    $.ajax({
        url: '/AssignP/ToggleView',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ Sections: sections, Type: type }),
        dataType: 'html',
        success: function (response) {

            document.getElementById('holder').innerHTML = response;

            console.log(response);
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);
            console.log(error);
            
        }
    });

}


function showAssignPopup(button) {

    var btn = button.closest("tr");

    var firstCell = btn.querySelector("td:first-child");
    //this needs to be fixed instead of prof
    prof = firstCell.textContent.trim();

    var popup = document.getElementById("popup");
    popup.classList.toggle("open-popup");
}

