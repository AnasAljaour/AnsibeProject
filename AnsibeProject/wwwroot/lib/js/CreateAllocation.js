var sections = [];
$(document).ready(function () {

    $('.Active-datatable').DataTable({
        // Define language settings
        language: {
            // Customize the text for when the table is empty
            emptyTable: "",
            zeroRecords: ""
        }
    });
});
function SaveList(button, CourseCode, CourseDescription, CourseHours, TP, TD) {
    var model = {
        SectionId: 0,
        Course: {
            CourseCode: CourseCode
        },
        Professor: null,
        TP: undefined,
        TD: undefined,
        CourseHours: undefined,
        Language: undefined
    };


    // Get the parent row of the button (i.e., the row containing the button)
    var tr = document.getElementById(CourseCode);

    // Get all checkboxes within the form
    var checkboxes = tr.querySelectorAll('input[type="checkbox"]');
    var option = tr.querySelector('select');
    var selectedText = option.options[option.selectedIndex].textContent;

    model.Language = option.selectedIndex

    // Filter checked checkboxes
    var checkedCheckboxes = [];
    var i = 0;
    checkboxes.forEach(function (checkbox) {
        checkedCheckboxes.push(checkbox.checked);

        if (checkbox.checked) {
            i++;
            checkbox.checked = false;
        }
    });

    model.CourseHours = (checkedCheckboxes[0]) ? parseInt(CourseHours) : null;
    model.TD = (checkedCheckboxes[1]) ? parseInt(TD) : null;
    model.TP = (checkedCheckboxes[2]) ? parseInt(TP) : null;


    if (i > 0) {


        // Create a new row and cells
        var newRow = document.createElement("tr");
        var newCell1 = document.createElement("td");
        var newCell2 = document.createElement("td");
        var newCell3 = document.createElement("td");
        var newCell4 = document.createElement("td");
        var newCell5 = document.createElement("td");
        var newCell6 = document.createElement("td");
        var newCell7 = document.createElement("td")
        // Add content to the new cells (e.g., text or input elements)
        // newCell1.classList.toggle("tr-section");
        newCell1.style.backgroundColor = "#0d6efd40";
        newCell2.style.backgroundColor = "#0d6efd40";
        newCell1.textContent = CourseCode;
        newCell2.textContent = CourseDescription;
        newRow.appendChild(newCell1);
        newRow.appendChild(newCell2);
        createCheckboxesWithStates(checkedCheckboxes, newRow);
        newCell6.style.backgroundColor = "#0d6efd40";
        newCell6.textContent = selectedText;

        // Append cells to the new row

        newRow.appendChild(newCell6);
        newCell7.style.backgroundColor = "0d6efd40"
        newRow.appendChild(newCell7);
        var btn = document.createElement("button");
        btn.type = "button";
        btn.textContent = "delete"
        btn.onclick = function () {
            confirmDelete(this);
        };
        btn.classList.toggle("inline-delete-btn");
        newCell7.appendChild(btn)
        // Insert the new row after the parent row
        tr.parentNode.insertBefore(newRow, tr.nextSibling);
        sections.push(model);
    }
    else {
        Swal.fire("at least one checkbox should be checked", "", "info");

    }
}
function createCheckboxesWithStates(checkboxStates, newRow) {


    // Clear existing checkboxes


    // Create new checkboxes based on saved states
    for (var i = 0; i < checkboxStates.length; i++) {
        var td = document.createElement('td');

        newRow.appendChild(td);
        var input = document.createElement('input');
        input.type = 'checkbox';
        input.checked = checkboxStates[i];
        input.disabled = true;
        td.style.backgroundColor = "#0d6efd40";
        td.appendChild(input);

        // You can set values or labels for the checkboxes as needed


    }

}
function DeleteSection(button) {
    
    var parentRow = button.parentNode.parentNode;
    columns = parentRow.getElementsByTagName('td');
    let courseCode = columns[0].textContent.trim();
    
    let checkboxes = parentRow.querySelectorAll('input[type="checkbox"]');

    var checkedCheckboxes = [];
    checkboxes.forEach(function (checkbox) {
        checkedCheckboxes.push(checkbox.checked);
    });
    

    let index = sections.findIndex(function (item) {
        if (item.Course.CourseCode.trim() !== courseCode) return false;
        
        if (checkedCheckboxes[2] && item.TP === null) return false;
        if (checkedCheckboxes[1] && item.TD === null) return false;
        if (checkedCheckboxes[0] && item.CourseHours === null) return false;
        let language = (columns[5].textContent === 'E') ? 0 : (columns[5].textContent === 'F') ? 1 : 2
        if (item.Language !== language) return false;

        return true;
    });
   
    if (index !== -1) {
        sections.splice(index, 1);
    }
   
    parentRow.innerHTML = "";

}


function confirmDelete(button) {
    Swal.fire({
        title: "Are you sure you want to delete this ?",
        text: "This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // If user confirms, perform the delete action
            DeleteSection(button)
        } else {
            // If user cancels, show an informational message
            Swal.fire("Deletion cancelled", "", "info");
        }
    });
}


function SubmitSection() {
    if (sections.length == 0) {
        Swal.fire("at least one section should be created", "", "info");
    }

    else {
        fetch('/Home/AddSection', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(sections)
        })
        .then(response => response.text())
        .then(data => {

            // Handle response if needed
            $('#partial-view').html(data)

        })
        .catch(error => {
            // Handle error
        });

        sections = []
    }
}
let prof;
let allocation = [];
function showAssignPopup(button) {

        var btn = button.closest("tr");

        var firstCell = btn.querySelector("td:first-child");

        prof = firstCell.textContent.trim();
    
    var popup = document.getElementById("popup");
    popup.classList.toggle("open-popup");
}



function saveAssign(button) {
    var tbody = document.getElementById("SectionId");
    var checkboxes = tbody.querySelectorAll('input[type="checkbox"]');


    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            // Remove the parent row of the checked checkbox
            var row = checkbox.closest('tr');


            var newRow = prepareRow(row);
            row.parentNode.removeChild(row);
            let courseTable = document.getElementById("body-" + prof);
            courseTable.appendChild(newRow);
        }
    });

    var popup = document.getElementById("popup");
    popup.classList.toggle("open-popup");

}


function prepareRow(row) {


    let newRow = document.createElement("tr");
    let columns = row.getElementsByTagName("td");
    for (let i = 0; i < columns.length - 1; i++) {
        var newcell = document.createElement("td");
        newcell.textContent = columns[i].textContent

        newRow.appendChild(newcell);

        if (i == columns.length - 2) {
            sectionId = columns[i].textContent;
            newcell.style.display = "none";
            addCourse(prof, sectionId);
        }
    }
    let cellForButton = document.createElement("td");


    let btn = document.createElement("button")
    btn.type = "button";
    btn.textContent = "delete"
    btn.onclick = function () {
        confirmDeleteAssignement(this);
    };
    btn.classList.toggle("inline-delete-btn");
    cellForButton.appendChild(btn);
    newRow.appendChild(cellForButton);


    return newRow
}


function addCourse(professoreId, sectionId) {
    let model = {
        Key: sectionId.toString(),
        Value: professoreId.toString()
    };
    allocation.push(model);

}

function submitAllocation() {
    console.log(JSON.stringify(allocation));
    $.ajax({
        url: '/Home/SaveAllocation',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(allocation),
        dataType: 'json',
        success: function (response) {

            if (response.success) {
                allocation = [];
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: "Your Allocation has been saved",
                    showConfirmButton: false,
                    timer: 1500
                });
                setTimeout(function () {
                    window.location.href = "/Home/Details?Id=" + response.id;
                }, 1500);
               
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.responseJSON && jqXHR.responseJSON.error) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Something went wrong! \n"  + jqXHR.responseJSON.error,
                    
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


        
        
    });





}

function confirmDeleteAssignement(button) {
    Swal.fire({
        title: "Are you sure you want to delete this ?",
        text: "This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // If user confirms, perform the delete action
            deleteAssignement(button)
        } else {
            // If user cancels, show an informational message
            Swal.fire("Deletion cancelled", "", "info");
        }
    });
}

function deleteAssignement(button) {
    let row = button.closest('tr');

    let columns = row.getElementsByTagName('td');

    newRow = document.createElement("tr");
    for (let i = 0; i < columns.length - 1; i++) {
        newcell = document.createElement("td");
        if (i == columns.length - 2) {
            let index = allocation.findIndex(item => item.Key === columns[i].textContent);
            if (index !== -1) {
                allocation.splice(index, 1);
            }
            newcell.style.display = "none";
        }
        newcell.textContent = columns[i].textContent
        newRow.appendChild(newcell);


    }
    let checkBox = document.createElement("input");
    checkBox.type = "checkbox";
    newcell = document.createElement("td");
    newcell.appendChild(checkBox);
    newRow.appendChild(newcell);
    tbody = document.getElementById("SectionId");
    tbody.appendChild(newRow);
    row.innerHTML = '';


}
function cancel() {

    let popup = document.getElementById("popup");
    let checkboxes = popup.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) checkbox.checked = false;
    });
    popup.classList.toggle("open-popup");
}



