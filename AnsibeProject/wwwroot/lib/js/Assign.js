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
let sections = [];
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
    if (type !== null &&sections != null && sections.length > 0) {
        console.log(sections);
        type = (type === "CP") ? "PS" : "CP";
        $.ajax({
            url: '/AssignP/ToggleView',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ Sections: sections, TempSections: null, Type: type }),
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
}






function showCreateSection() {
    let popup = document.getElementById('popup');
    popup.classList.toggle('open-popup');

}
let tempSections = []

function createSections(button, CourseCode, CourseDescription, CourseHours, TP, TD) {
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

    let row = button.closest("tr");
    let checkboxes = row.querySelectorAll('input[type="checkbox"]');
    console.log(checkboxes);
    let option = row.querySelector('select');
    var selectedText = option.options[option.selectedIndex].textContent;

    model.Language = option.selectedIndex

    let checkedCheckboxes = [];
    let i = 0;
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
        let newRow = document.createElement('tr');

        let courseCell = document.createElement('td');
        courseCell.textContent = model.CourseHours;

        let TPCell = document.createElement('td');
        TPCell.textContent = model.TP;

        let TDCell = document.createElement('td');
        TDCell.textContent = model.TD;

        let languageCell = document.createElement('td');
        languageCell.textContent = selectedText

        let deleteButtonCell = document.createElement('td');
        let deleteButton = document.createElement('button');
        deleteButton.type = "button";
        deleteButton.textContent = "delete";
        deleteButton.onclick = function () {

        };

        deleteButton.classList.toggle("inline-delete-btn");
        deleteButtonCell.appendChild(deleteButton);


        newRow.appendChild(courseCell);
        newRow.appendChild(TPCell);
        newRow.appendChild(TDCell);
        newRow.appendChild(languageCell);
        newRow.appendChild(deleteButtonCell);

        let tbody = document.getElementById('tbody-' + model.Course.CourseCode);
        tbody.appendChild(newRow);
        tempSections.push(model);


    }
    else {
        Swal.fire("at least one checkbox should be checked", "", "info");

    }

}

function cancelCreation() {
    let popup = document.getElementById('popup');
    popup.classList.toggle('open-popup');
    tempSections = [];

    let tbodyElements = popup.querySelectorAll('tbody[id^="tbody-"]');
    tbodyElements.forEach(function (tbody) {
        tbody.innerHTML = '';
    });
}

function saveCreatedSections() {
    if (tempSections.length > 0) {
        $.ajax({
            url: '/AssignP/SaveCreatedSections',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ Sections: sections, TempSections: tempSections, Type: type }),
            dataType: 'html',
            success: function (response) {

                document.getElementById('holder').innerHTML = response;
                
                
                $.ajax({
                    url: '/AssignP/getCreatedSections',
                    type: 'POST',
                    dataType: 'json',
                    success: function (response) {

                        sections = response;
                        
                        console.log(response);
                    },
                    error: function (xhr, status, error) {
                        failed(xhr, status, error);
                        document.getElementById('holder').innerHTML = '';
                        

                    }
                });
                
            },
            error: function (xhr, status, error) {
                failed(xhr, status, error);


            }
        });
        if (type == null) type = "CP";
        cancelCreation()
        
        
    }
    else {
        Swal.fire("at least one section should be created", "", "info");
    }

}


function showAssignPopup(button) {

    var btn = button.closest("tr");

    var firstCell = btn.querySelector("td:first-child");
    //this needs to be fixed instead of prof
    prof = firstCell.textContent.trim();

    var popup = document.getElementById("popup");
    popup.classList.toggle("open-popup");
}

function showAssignPopup1(button) {

    var btn = button.closest("tr");

    var firstCell = btn.querySelector("td:first-child");
    //this needs to be fixed instead of prof
    prof = firstCell.textContent.trim();

    var popup = document.getElementById("popup1");
    popup.classList.toggle("open-popup");
}

function cancelCreation1() {
    let popup = document.getElementById('popup1');
    popup.classList.toggle('open-popup');
    tempSections = [];

    let tbodyElements = popup.querySelectorAll('tbody[id^="tbody-"]');
    tbodyElements.forEach(function (tbody) {
        tbody.innerHTML = '';
    });
}

let lastCell
function showAssignProfessors(button) {
    var btn = button.closest("tr");
     lastCell = btn.querySelector("td:last-child");
    var popup = document.getElementById("popupProfessors");
    popup.classList.toggle("open-popup");
}

function AssignProfessorInCourse(button) {
    var btn = button.closest("tr");
    var firstCell = btn.querySelector("td:first-child");
    var beforeLastTd = lastCell.previousElementSibling;
    beforeLastTd.textContent = firstCell.nextElementSibling.textContent;
    btn.parentNode.removeChild(btn);
    var popup = document.getElementById("popupProfessors");
    popup.classList.toggle("open-popup");
}
