import pandas as pd
import numpy as np
import random
random.seed(0)
np.random.seed(0)
class Decision():
    def __init__(self, feature, threshold):
        """
        Decision class

        Feature: the index of the feature (index based on the input x data)
        threshold: the number which decide what values go to the left/right child of the current node

        """
        self.feature = feature
        self.threshold = threshold


class Tree():
    def __init__(self, root):
        """
        Tree class, intialize a Tree object to be store the root node.

        root: First TreeNode object in the tree
        """
        self.root = root


class TreeNode():
    def __init__(self, values, left_child=None, right_child=None, leaf=False):
        """
        TreeNode class, intialize a TreeNode object

        values: The input data (predictors and labels)
        left_child: TreeNode object which is the left child of the current node
        right_child: TreeNode object which is the right child of the current node
        leaf: bool which indicates that the current node is a leaf or not
        labels: array of the labels in the node
        decision: Decision object of the node
        """
        self.values = values
        self.left_child = left_child
        self.right_child = right_child
        self.leaf = leaf
        self.labels = values.transpose()[-1].astype(int)
        self.decision = None

    def gini_index(self):
        """
        Calculates the gini impurity of the current node
        ----------------------------------------------------------------------------------------------------
        returns the gini impurity (int)
        """
        serie = pd.Series(self.labels)
        summ = 0
        for i in serie.value_counts():
            summ = summ + np.square(i / len(serie))
        gini_impurity = 1 - summ
        return gini_impurity

    def majority(self):
        """
        returns the most common label in the node
        ----------------------------------------------------------------------------------------------------
        returns the marjority label in the node (int 0 or 1)
        """
        counter = np.bincount(self.labels)
        return np.argmax(counter)


class ClassificationTree():

    def tree_grow(self, x, y, nmin, minleaf, nfeat):
        """
        This function grows a single tree on x and y
        ----------------------------------------------------------------------------------------------------
        X: a data matrix (2-dimensional array) containing the attribute values. Each row of x contains the 
        attribute values of one training example. 
        
        Y: vector (1-dimensional array) of class labels. The class label is binary, with values coded as 0 and 1.
        
        nmin: the number (int) of observations that a node must contain at least, for it to be allowed to be split.
        
        minleaf: the minimum number (int) of observations required for a leaf node
        
        nfeat: the number (int) of features that should be considered for each split.
        ----------------------------------------------------------------------------------------------------
        returns a tree object
        
        """
        # Create a root node with the data, then create a tree with the root node attached
        data = TreeNode(np.concatenate((x, np.vstack(y)), axis=1))
        DecisionTree = Tree(data)
        nodelist = [data]
        while (len(nodelist) > 0):
            # If not all nodes in the stack are considerd, take the first node to be considered of the stack
            node = nodelist.pop()
            if len(node.values) >= nmin and node.gini_index() > 0 and node.leaf == False:
                # Given values in nodes >= nmin, node is not pure and not a leaf create splits on the values
                S = self.create_splits(node.values, nfeat, len(node.values[0]), minleaf)
                impurities = []
                index = 0
                # Calculate impurity reductions for each split
                for split in S:
                    impurities.append(impurity_reduction(node, split))

                # If there are splits where amount of values in child nodes are not < nleaf
                # Select best split and create decision for current node
                # Create left and right children for current node using the best split
                # Add child nodes to the stack (DFS)
                if len(impurities) > 0:
                    argmax_split_left = S[np.argmax(impurities)][0]
                    argmax_split_right = S[np.argmax(impurities)][1]
                    argmax_feature = S[np.argmax(impurities)][2]
                    threshold = (argmax_split_left[-1][argmax_feature] + argmax_split_right[0][argmax_feature]) / 2
                    node.left_child = TreeNode(argmax_split_left)
                    node.right_child = TreeNode(argmax_split_right)
                    node.decision = Decision(argmax_feature, threshold)
                    nodelist.append(node.right_child)
                    nodelist.append(node.left_child)

                else:
                    node.leaf = True

            else:
                node.leaf = True

        return DecisionTree

    def tree_pred(self, X, tr):
        """
        X: a data matrix (2-dimensional array) containing the attribute values. Each row of x contains the 
        attribute values of one sample.
        
        This function predicts a value y_pred for each value x in X, using a single tree tr
        ----------------------------------------------------------------------------------------------------
        tr: a tree object created with the function tree_grow.
        ----------------------------------------------------------------------------------------------------
        returns a list of predictions

        """
        # Go to tree to find classification for each array in X
        predictions = []
        for x in X:
            # Start from the root node and while no leaf node is found explore more (DFS)
            nodelist = [tr.root]
            Found = False
            while Found == False:
                node = nodelist.pop()
                if node.leaf == True:
                    # Leaf node found
                    predictions.append(node.majority())
                    Found = True
                elif x[node.decision.feature] <= node.decision.threshold:
                    # Explore left child if x value <= threshold
                    nodelist.append(node.left_child)
                else:
                    # Explore right child if x value > threshold
                    nodelist.append(node.right_child)
        return predictions

    """
    !!! Very slow function for growing large amount of trees. !!!
    """

    def tree_grow_b(self, x, y, nmin, minleaf, nfeat, m):
        """
        The function grows m trees on x and y as input using bagging.
        ----------------------------------------------------------------------------------------------------
        X: a data matrix (2-dimensional array) containing the attribute values. Each row of x contains the 
        attribute values of one training example. 
        
        Y: vector (1-dimensional array) of class labels. The class label is binary, with values coded as 0 and 1.
        
        nmin: the number (int) of observations that a node must contain at least, for it to be allowed to be split.
        
        minleaf: the minimum number (int) of observations required for a leaf node
        
        nfeat: the number (int) of features that should be considered for each split.
        
        m: number (int) of bootstrap samples to be drawn
        ----------------------------------------------------------------------------------------------------
        returns a list of trees
       
        """
        trees = []
        rng = np.random.default_rng()
        data = np.concatenate((x, np.vstack(y)), axis=1)
        # Grow m amount of trees
        for i in range(m):
            print(f"Constructing tree {i}/{m}")
            # Randomly sample the data with replacement
            data_sample = rng.choice(data, len(data), replace=True)
            x_sample = data_sample.T[:-1].T
            y_sample = data_sample.T[-1].T
            # Grow the tree
            trees.append(self.tree_grow(x_sample, y_sample, nmin, minleaf, nfeat))
        return trees

    def tree_pred_b(self, x, trs):
        """
        The function applies tree_pred to x using each tree in the list in turn. For each row of x the final
        prediction is obtained by taking the majority vote of the m predictions. The function returns a vector y_pred,
        where y_pred[i] contains the predicted class label for row i of x.
        ----------------------------------------------------------------------------------------------------
        X: a data matrix (2-dimensional array) containing the attribute values. Each row of x contains the 
        attribute values of one sample.

        trs: a list of tree objects created with the function tree_grow.
        ----------------------------------------------------------------------------------------------------
        returns a list of predictions
        """
        y_pred = []
        # Get predictions of all trees on all x values
        for tree in trs:
            tree_pred = self.tree_pred(x, tree)
            y_pred.append(tree_pred)

        final_pred = []
        # Get majority of each prediction on the x values
        for i in np.array(y_pred).transpose():
            counter = np.bincount(i)
            final_pred.append(np.argmax(counter))

        return final_pred

    def create_splits(self, values, nfeat, predictors, minleaf):
        """
        This function brute forces all possible splits give an array of values
        ----------------------------------------------------------------------------------------------------
        values: a list of the input data
         
        nfeat: the number (int) of features that should be considered for each split.
        
        predictors: the amount of available predictors (int)
        
        minleaf: the minimum number (int) of observations required for a leaf node
        ----------------------------------------------------------------------------------------------------
        returns a list of all possible splits
        
        """
        S = []
        indexes = [i for i in range(len(values[0]) - 1)]

        # Shuffle the features if not all have to be considered
        if predictors != nfeat:
            random.shuffle(indexes)

        # Create numerical splits on the given values
        for idx in indexes[:nfeat]:
            sort = values[values[:, idx].argsort()]
            for i in range(1, len(sort)):
                if sort[i - 1][idx] != sort[i][idx] and len(sort[:i]) >= minleaf and len(sort[i:]) >= minleaf:
                    S.append((sort[:i], sort[i:], idx))
        return S


def gini_index(labels):
    """
    This functions calculates the gini impurity given a list of labels
    ----------------------------------------------------------------------------------------------------
    labels: a list of values (values 0 or 1)
    ----------------------------------------------------------------------------------------------------
    returns a gini impurity value as int
    """
    #Calculate the gini impurity given an array
    serie = pd.Series(labels)
    summ = 0
    for i in serie.value_counts():
        summ =  summ + np.square(i / len(serie))
    gini_impurity = 1 - summ
    return gini_impurity

def weighted_gini(s1,s2):
    """
    This functions calculates the weighted gini given two list of labels
    ----------------------------------------------------------------------------------------------------
    s1: First list of values (values 0 or 1)
    s2: Second list of labels (values 0 or 1)
    ----------------------------------------------------------------------------------------------------
    returns the weighted gini impurity
    """
    return (len(s1)*gini_index(s1) + len(s2)*gini_index(s2)) / (len(s1)+len(s2))

def impurity_reduction(node, split):
    """
    This functions calculates the gini impurity reduction.
    ----------------------------------------------------------------------------------------------------
    node: the node which will be split
    split: the split that will be chosen
    ----------------------------------------------------------------------------------------------------
    returns the gini impurity reduction
    """
    #Calculate the gini impurity reduction give the root node and its two children given a split
    return node.gini_index() - (weighted_gini(split[0].transpose()[-1],split[1].transpose()[-1]))



#Sample Code for growing a tree on pima.csv
"""
import pandas as pd
from sklearn.metrics import confusion_matrix, ConfusionMatrixDisplay
import matplotlib.pyplot as plt

df = pd.read_csv('pima.csv',names=['Pregnancies','Glucose','BloodPressure','SkinThickness','Insulin','BMI','DiabetesPedigreeFunction','Age','Outcome'])
X_train = df.drop(columns = ['Outcome'])
y_train = df['Outcome']
X_train = X_train.to_numpy()
y_train = y_train.to_numpy()
df = df.to_numpy()

CLF = ClassificationTree()
CLF_tree = CLF.tree_grow(x = X_train, y = y_train, nmin = 20, minleaf = 5, nfeat =9)

y_pred_single = CLF.tree_pred(X_train,CLF_tree)
y_pred_train = CLF.tree_pred(X_train,CLF_tree)
cm = confusion_matrix(y_train, y_pred_single)
disp = ConfusionMatrixDisplay(confusion_matrix=cm)
disp.plot()
plt.show()



"""