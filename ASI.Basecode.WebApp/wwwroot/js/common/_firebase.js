import { initializeApp } from "https://www.gstatic.com/firebasejs/11.9.1/firebase-app.js";
import { getStorage, ref, uploadBytesResumable, getDownloadURL, deleteObject } from "https://www.gstatic.com/firebasejs/11.9.1/firebase-storage.js";

const firebaseConfig = {
    apiKey: "AIzaSyA4CTMSbgGQN_yLn9lEZlswbZ_2A2Xhl0k",
    authDomain: "basabuzz-ca8fe.firebaseapp.com",
    projectId: "basabuzz-ca8fe",
    storageBucket: "basabuzz-ca8fe.firebasestorage.app",
    messagingSenderId: "206533484485",
    appId: "1:206533484485:web:2c71a06a17d5244efe75ac"
};

const app = initializeApp(firebaseConfig);
const storage = getStorage(app);

export async function deleteFileFromFirebase(fileUrl) {
    if (!fileUrl || fileUrl.includes("firebaseapp.com/o/book_covers%2Fplaceholder.jpg") || fileUrl.includes("firebaseapp.com/o/book_files%2Fplaceholder.pdf")) {
        console.warn("Attempted to delete null, empty, or placeholder file URL.");
        return;
    }
    try {
        const fileRef = ref(storage, fileUrl);
        await deleteObject(fileRef);
        console.log(`Successfully deleted file: ${fileUrl}`);
    } catch (error) {
        toastr.error(`Failed to delete file ${fileUrl}: ${error.message}`);
        console.error(`Failed to delete file ${fileUrl}:`, error);
    }
}

export async function uploadFileToFirebase(file, path, statusElement) {
    if (!file) {
        statusElement.textContent = 'No file selected.';
        return null;
    }

    const MAX_IMAGE_FILE_SIZE_BYTES = 10 * 1024 * 1024;
    const MAX_BOOK_FILE_SIZE_BYTES = 100 * 1024 * 1024;

    let maxAllowedSize = 0;
    if (path.includes('book_covers')) {
        maxAllowedSize = MAX_IMAGE_FILE_SIZE_BYTES;
    } else if (path.includes('book_files')) {
        maxAllowedSize = MAX_BOOK_FILE_SIZE_BYTES;
    }

    if (file.size > maxAllowedSize) {
        const errorMessage = `File size exceeds the maximum allowed limit of ${maxAllowedSize / (1024 * 1024)}MB.`;
        statusElement.textContent = errorMessage;
        toastr.error(errorMessage);
        return null;
    }

    const timestamp = new Date().getTime();
    const randomString = Math.random().toString(36).substring(2, 8);
    const uniqueFileName = `${path}${timestamp}-${randomString}-${file.name}`;
    const storageRef = ref(storage, uniqueFileName);
    const uploadTask = uploadBytesResumable(storageRef, file);

    return new Promise((resolve, reject) => {
        uploadTask.on('state_changed',
            (snapshot) => {
                const progress = (snapshot.bytesTransferred / snapshot.totalBytes) * 100;
                statusElement.textContent = `Upload is ${progress.toFixed(2)}% done`;
            },
            (error) => {
                statusElement.textContent = `Upload failed: ${error.message}`;
                console.error("Upload error:", error);
                toastr.error(`File upload failed: ${error.message}`);
                reject(error);
            },
            async () => {
                try {
                    const downloadURL = await getDownloadURL(uploadTask.snapshot.ref);
                    statusElement.textContent = 'Upload successful!';
                    resolve(downloadURL);
                } catch (error) {
                    statusElement.textContent = `Failed to get download URL: ${error.message}`;
                    console.error("Get Download URL error:", error);
                    toastr.error(`Failed to get download URL: ${error.message}`);
                    reject(error);
                }
            }
        );
    });
}

